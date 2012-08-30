// File name: MainWindow.xaml.cs
// Creator: Jonatan Pålsson
// Solution: DeckOfCards
// Project: WPFTest
// Creation date: 2012-08-22-5:20 PM
// 
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using CardLogic;
using Cards;
using Networking;
using Networking.Exceptions;
using Networking.Messages;

namespace WPFTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly StackObj p1Stack;

        public MainWindow()
        {
            InitializeComponent();
            p1Stack = new StackObj();
            player1_stack1.ConnectStack(p1Stack);
            player1_stack1.SetShowBack(false);
            RegisterListeners();
        }

        private void RegisterListeners()
        {

            /*
            var player1Receiver = new Receive();
            player1Receiver.SetupConsumer("192.168.1.68", 5672, "guest","guest", "1.1.1");
            player1Receiver.RegisterConsumer(obj =>
                                             {
                                                 return true;
                                             });
             */


            var player2Receiver = new Receive();
            try
            {
                player2Receiver.SetupConsumer("192.168.1.68", 5672, "guest", "guest", "1.1.1");
                player2Receiver.RegisterConsumer(obj =>
                                                     {
                                                         Player2HandleIncomingMessage(obj);
                                                         return true;
                                                     });
            }
            catch (ConnectionFailedException e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            StackObj stack = p1Stack.GetSerializableCopy();
            var msg = new StackInitializeMessage
                          {NewStack = stack, OriginFriendlyName = "Player 1", OriginId = Guid.NewGuid()};
            sendMessage(msg, "1.1.1");
        }

        private void sendMessage(object message, string routeKey)
        {
            var sendObj = new Send();
            sendObj.CreateConnection("192.168.1.68", 5672, "guest", "guest");
            sendObj.SendSerializableObject(message, routeKey);
        }

        private void drawButton_Click(object sender, RoutedEventArgs e)
        {
            sendMessage(
                new DrawMessage {DrawFrom = "stack", OriginFriendlyName = "Player 1", OriginId = Guid.NewGuid()},
                "1.1.1");
            PlaceNewCardOnBoard(1, player1_stack1.SpawnClonedCard());
        }

        private void Player2HandleIncomingMessage(object message)
        {
            if (message is StackInitializeMessage)
            {
                var newStack = (message as StackInitializeMessage).NewStack;
                var action = new Action(delegate()
                                            {
                                                player2_stack1.ConnectStack(newStack);
                                                player2_stack1.SetShowBack(false);
                                            });
                player2_stack1.Dispatcher.Invoke(DispatcherPriority.Normal, action);
            }
            else if (message is DrawMessage)
            {
                var drawMessage = (message as DrawMessage);
                if (drawMessage.DrawFrom.Equals("stack"))
                {
                    var action = new Action(delegate
                                                {
                                                    var co = player2_stack1.SpawnClonedCard();
                                                    PlaceNewCardOnBoard(2, co);
                                                    player2_stack1.SetShowBack(false);
                                                });
                    player2_stack1.Dispatcher.Invoke(DispatcherPriority.Normal, action);
                }
            }
            else if (message is MoveMessage)
            {
                var action = new Action(delegate
                                            {
                                                var mm = message as MoveMessage;
                                                var c = player2_board.Children[0] as Card;
                                                if (mm.Type == MoveMessage.MoveType.Relative)
                                                    c.RelativeMove(mm.Where.X, mm.Where.Y);
                                                else if (mm.Type == MoveMessage.MoveType.Absolute)
                                                    c.AbsoluteMove(mm.Where.X, mm.Where.Y);
                                            });
                player2_stack1.Dispatcher.Invoke(DispatcherPriority.Normal, action);
            }

            else
            {
                MessageBox.Show("Unknown message type");
            }
        }

        private void PlaceNewCardOnBoard(int player, Card c)
        {
            if (player == 1)
            {
                player1_board.Children.Add(c);
            }
            else
            {
                player2_board.Children.Add(c);
            }
            //Canvas.SetLeft(c, 0);
            //Canvas.SetTop(c,0);
            c.Height = 30;
            c.Width = 30;
            c.FinishedMoving += CardMovedListener;
        }

        private void CardMovedListener(object card, Point newPosition)
        {
            var offenderId = (card as Card).CardObject.Id;
            var np = new System.Drawing.Point((int) newPosition.X, (int) newPosition.Y);
            sendMessage(new MoveMessage{Type = MoveMessage.MoveType.Absolute, What = offenderId, Where = np}, "1.1.1");
        }

        private void moveCard_Click(object sender, RoutedEventArgs e)
        {
            var id = (player1_board.Children[0] as Card).CardObject.Id;
            sendMessage(new MoveMessage{Comment = "Moving card", What = id, Where = new System.Drawing.Point(50,50), OriginId = Guid.NewGuid(), OriginFriendlyName = "Player1"}, "1.1.1");
        }
        }

    }
