// File name: Receive.cs
// Creator: Jonatan Pålsson
// Solution: DeckOfCards
// Project: Networking
// Creation date: 2012-08-20-4:57 PM
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Networking.Exceptions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Networking
{
    public class Receive
    {
        private static IConnection _connection;
        private static IModel _channel;
        private Dictionary<string, IModel> _channels;
        private Dictionary<string, QueueingBasicConsumer> _consumers;

        private const string ExchangeName = "topic-exchange";
        private static string _queue = "objects";

        public static void SetupConsumer(string hostname, int port, string username, string password, string routingKey)
        {
            try
            {
                var factory = new ConnectionFactory
                                  {
                                      HostName = hostname,
                                      Port = port,
                                      UserName = username,
                                      Password = password
                                  };
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic);
                _queue = _channel.QueueDeclare();
                _channel.QueueBind(_queue, ExchangeName, routingKey);
            }catch(BrokerUnreachableException e)
            {
                throw new ConnectionFailedException("The connection has failed to establish: " + e.ToString());
            }
        }


        /// <summary>
        /// Spawn a thread which will call the specified callback upon receiving a message
        /// </summary>
        /// <param name="callback"></param>
        public static void RegisterConsumer(Func<object, bool> callback)
        {

            // Create a consumer
            var consumer = new QueueingBasicConsumer(_channel);
            _channel.BasicConsume(_queue, true, consumer);

            var t = new Thread(() =>
                                   {
                                       while (true)
                                       {
                                           try
                                           {
                                               using (var ms = new MemoryStream())
                                               {
                                                   var eventArgs = consumer.Queue.Dequeue() as BasicDeliverEventArgs;

                                                   // copy data into memorystream, so we can deserialize
                                                   ms.Write(eventArgs.Body, 0, eventArgs.Body.Length);
                                                   ms.Position = 0; // position is end of stream, reset

                                                   object deserialized = new BinaryFormatter().Deserialize(ms);

                                                   callback(deserialized);
                                               }
                                           }
                                           catch (EndOfStreamException)
                                           {
                                               break;
                                           }
                                       }
                                   });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        public static string GenerateRouteKey(string text)
        {
            return "1.1." + text;
        }
    }
}
