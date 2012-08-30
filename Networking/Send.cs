// File name: Send.cs
// Creator: Jonatan Pålsson
// Solution: DeckOfCards
// Project: Networking
// Creation date: 2012-08-20-4:02 PM
// 
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing.v0_9_1;
using RabbitMQ.Client.Exceptions;
using Networking.Exceptions;

namespace Networking
{
    public class Send
    {
        private static IConnection _connection;
        private static IModel _sendChannel;
        private static bool _connectionEstablished;

        private const string Exchange = "topic-exchange";

        public static bool ConnectionEstablished
        {
            get { return _connectionEstablished; }
        }

        public static bool CreateConnection(string hostname,int port, string username, string password)
        {
            var factory = new ConnectionFactory
            {
                HostName = hostname,
                Port = port,
                UserName = username,
                Password = password,
                VirtualHost = "/"
            };

            try
            {
                //TODO: Catch exception BrokerUnreachableException
                _connection = factory.CreateConnection();
                _sendChannel = _connection.CreateModel();
                _sendChannel.ExchangeDeclare(Exchange, ExchangeType.Topic);
                _connectionEstablished = true;
                return _connection.IsOpen;
            }
            catch (BrokerUnreachableException e) {
                throw new ConnectionFailedException("Unable to reach host");
            }
        }

        public static void SendSerializableObject(object o, string routingKey)
        {
            using (var ms = new MemoryStream())
            {
                new BinaryFormatter().Serialize(ms, o);
                _sendChannel.BasicPublish(Exchange,routingKey,null, ms.ToArray());
            }

        }

        public static string GenerateRouteKey(string text)
        {
            return "1.1." + text;
        }
    }
}
