// File name: MainWindow.xaml.cs
// Creator: Jonatan Pålsson
// Solution: DeckOfCards
// Project: DeckOfCards
// Creation date: 2012-08-08-12:32 AM
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using CardLogic;
using Cards;
using Networking;
using Networking.Messages;
using Networking.Exceptions;


namespace DeckOfCards
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// The scaler class is used to register any WPF UserClass for scaling when the window
        /// size changes.
        /// </summary>
        private readonly CardScaler _scaler ;

        /// <summary>
        /// The constructor for MainWindow contains a *lot* of event binds, many of these things could have been
        /// done in XAML, but since this is a lab on events.. I did a few event binds.
        /// </summary>
        public MainWindow(string[] param)
        {
            InitializeComponent();

            // Load potential command-line parameters
            if (param.Length > 2)
            {
                txtMyNodeName.Text = param[0];
                txtTheirNodeName.Text = param[1];
                txtHostName.Text = param[2];
            }

            // The control is not yet loaded (and thus hasn't allocated screen estate) when this is 
            // run, so hook into the Loaded event to make sure the actuals can be accessed.
            _scaler = new CardScaler(sclScale.Value/100);

            // Create a new stack
            InitNewStack();

            // When the window has finished rendering, set a size to the scaler so that it can begin working
            //ContentRendered +=
            //    (sender, args) => _scaler.OnScale(new Size(CardSpawn.ActualWidth, CardSpawn.ActualHeight));

            // When the draw button is clicked, draw a new card. Pass along whether to draw one or all cards
            btnDraw.Click += (sender, args) => NetworkedDrawNewCard(rdoShowAllCards.IsChecked != null && rdoShowAllCards.IsChecked.Value);

            // When the size of the cardSpawn changes, pass this along to the scaler
            //CardSpawn.SizeChanged += (sender, args) => _scaler.OnScale(args.NewSize);

            // When the chkSHowCardsFaceUp checkbox is set, make the corresponding events 
            // trigger appropriate actions in the stack
            chkShowCardsFaceUp.Checked += (sender, args) => stack11.SetShowBack(false);
            chkShowCardsFaceUp.Unchecked += (sender, args) => stack11.SetShowBack(true);

            // Helper for toggling drawing settings
            Func<bool, bool> toggle = isToggle =>
            {
                rdoLine.IsEnabled = isToggle;
                rdoSine.IsEnabled = isToggle;
                rdoRandom.IsEnabled = isToggle;
                return isToggle;
            };

            // When all cards should be drawn, unlock some options in the GUI (Func above)
            rdoShowAllCards.Checked += (sender, args) => toggle(true);
            rdoShowAllCards.Unchecked += (sender, args) => toggle(false);

        }
        private void NetworkedInitNewStack()
        {
            InitNewStack();
            if (Send.ConnectionEstablished)
                Send.SendSerializableObject(new StackInitializeMessage { NewStack = stack11.Stack.GetSerializableCopy() },
                                        Send.GenerateRouteKey(txtTheirNodeName.Text));
        }

        /// <summary>
        /// Initialize a new stack, this involves binding some events, watching the stack with the scaler
        /// (so that it resizes properly), and positioning the stack in the canvas.
        /// </summary>
        private void InitNewStack(StackObj replacement = null)
        {
            StackObj stack = null;
            if (replacement == null)
                stack = new StackObj();
            else
                stack = replacement;    
            stack11 = new Stack1();
            stack11.ConnectStack(stack);
            CardSpawn.Children.Add(stack11);
            _scaler.Watch(stack11);
            
            btnNewStack.Click += (sender, args) =>
                                     {
                                         stack.Destroy();
                                         CardSpawn.Children.Remove(stack11);
                                         CardSpawn.Children.Clear();
                                         NetworkedInitNewStack();
                                         lstAces.Items.Clear();
                                     };
            stack11.card1.MouseDown += (sender, args) => NetworkedDrawNewCard(false);
        }

        private void NetworkedDrawNewCard(bool drawAll)
        {
            if (Send.ConnectionEstablished)
            {
                Send.SendSerializableObject(new DrawMessage {DrawAll=drawAll}, Send.GenerateRouteKey(txtTheirNodeName.Text));
            }
            DrawNewCard(drawAll);

        }

        /// <summary>
        /// Draw a new card, this involves, when drawing all cards, the selection of a placement "algorithm"
        /// calculating the allowed bounds for placement, adding the newly drawn card to a list of it is an
        /// ace, etc..
        /// </summary>
        /// <param name="drawAll"></param>
        private void DrawNewCard(bool drawAll)
        {
            var rng = new Random();
            int i = 0;
            do
            {
                var card = stack11.SpawnClonedCard();
                if (card == null)
                    break;
                if (card.CardObject.Value == 1)
                    lstAces.Items.Add(card.ToString());
                card.CardObject.ShowBack = false;
                CardSpawn.Children.Add(card);
                _scaler.Watch(card);


                // Track the movement of the new card
                card.FinishedMoving +=
                    (sender, position) =>
                        {
                            if (Send.ConnectionEstablished)
                                Send.SendSerializableObject(
                                    new MoveMessage
                                        {
                                            Type = MoveMessage.MoveType.Absolute,
                                            Where = position,
                                            What = card.CardObject.Id
                                        },
                                    Send.GenerateRouteKey(txtTheirNodeName.Text));
                        };

                if (drawAll)
                {
                    var maxX = CardSpawn.ActualWidth - card.ActualWidth;
                    var maxY = CardSpawn.ActualHeight - card.ActualHeight;
                    if (rdoSine.IsChecked != null && rdoSine.IsChecked.Value)
                        PositionCardSine(card, i, maxX, maxY);
                    if (rdoLine.IsChecked != null && rdoLine.IsChecked.Value)
                        PositionCardsInLine(card, i, maxX, maxY);
                    if (rdoRandom.IsChecked != null && rdoRandom.IsChecked.Value)
                        PositionCardRandomly(card, rng, maxX, maxY);
                }
                i++;
            } while (drawAll && stack11.HasMoreCards());
        }

        /// <summary>
        /// This is a placement strategy when drawing all cards. This strategy will randomly spread all the cards
        /// over the canvas.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="rng"></param>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        private static void PositionCardRandomly(UserControl card, Random rng, double maxX, double maxY)
        {
            //Canvas.SetLeft(card, rng.Next((int) maxX));
            //Canvas.SetTop(card, rng.Next((int) maxY));
        }

        /// <summary>
        /// Spread the cards according to a sine curve, making a wave-y pattern
        /// </summary>
        /// <param name="card"></param>
        /// <param name="i"></param>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        private static void PositionCardSine(UserControl card, double i, double maxX, double maxY)
        {
            var offsetY = (int) (maxY/2);
            var modX = maxX / 45;
            const double modY = 30;
            //Canvas.SetTop(card, Math.Sin(i)*modY + offsetY);
            //Canvas.SetLeft(card, i*modX);
            (card as Card).AbsoluteMove(i*modX, Math.Sin(i) * modY + offsetY);
        }

        /// <summary>
        /// Position all cards in a straight line
        /// </summary>
        /// <param name="card"></param>
        /// <param name="i"></param>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        private static void PositionCardsInLine(UserControl card, double i, double maxX, double maxY)
        {
            var offsetY = (int) (maxY/2);
            var modX = maxX / 45;
            //Canvas.SetTop(card, offsetY);
            //Canvas.SetLeft(card, i*modX);

        }

        /// <summary>
        /// Class for scaling UI elements when the window size changes. It allows adding/removing cards
        /// and allows for triggering re-scaling accoring to predefined scaling numbers (default 33%).
        /// </summary>

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Send.CreateConnection(
                    txtHostName.Text,               // Hostname
                    Convert.ToInt16(txtPort.Text),  // Port
                    "guest",                        // Username
                    "guest"                         // Password
                    );

                Send.SendSerializableObject(new StackInitializeMessage { NewStack = stack11.Stack.GetSerializableCopy() }, 
                    Send.GenerateRouteKey(txtTheirNodeName.Text));

                Receive.SetupConsumer(
                    txtHostName.Text,                           // Hostname
                    Convert.ToInt16(txtPort.Text),              // Port
                    "guest",                                    // Username
                    "guest",                                    // Password
                    Receive.GenerateRouteKey(txtMyNodeName.Text)// Incoming routing key (our "name")
                    );

                Receive.RegisterConsumer(o =>
                {
                    var action = new Action(() => HandleIncomingMessage(o));
                    this.Dispatcher.Invoke(DispatcherPriority.Normal, action);
                    return true;
                });
                txtMyNodeName.IsEnabled = false;
                txtTheirNodeName.IsEnabled = false;
                btnConnect.IsEnabled = false;
                txtHostName.IsEnabled = false;
                txtPort.IsEnabled = false;
            }
            catch (ConnectionFailedException ex)
            {
                MessageBox.Show("Error: \"" + ex.Message + "\", the system will \nattempt to gracefully degrade to non-networked mode.");
            }

        }

        private void txtChatInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                var message = new ChatMessage{MessageText = txtChatInput.Text, OriginFriendlyName = txtMyNodeName.Text};
                Send.SendSerializableObject(message, Send.GenerateRouteKey(txtTheirNodeName.Text));
                Send.SendSerializableObject(message, Send.GenerateRouteKey(txtMyNodeName.Text));
                txtChatInput.Text = "";
            }
        }

        private void HandleIncomingMessage(object message)
        {
            if (message is ChatMessage)
            {
                var m = message as ChatMessage;
                AddChatLine(m.OriginFriendlyName, m.MessageText);
            }
            else if (message is MoveMessage)
            {
                var m = message as MoveMessage;
                var c = LookupCardByGuid(m.What);
                if (c != null)
                {
                    c.AbsoluteMove(m.Where.X, m.Where.Y);
                    AddChatLine("SYSTEM", "Found card");
                }
                else { AddChatLine("SYSTEM", "Didn't find card!"); }
            }
            else if (message is DrawMessage)
            {
                var m = (message as DrawMessage);
                DrawNewCard(m.DrawAll);
            }
            else if (message is StackInitializeMessage)
            {
                var m = message as StackInitializeMessage;
                InitNewStack(m.NewStack);
            }
        }

        private Card LookupCardByGuid(Guid g)
        {
            var card = from r in CardSpawn.Children.OfType<Card>()
                       where r.CardObject.Id.Equals(g)
                       select r;
            if (card.Any())
                return card.First();
            return null;
        }

        private void AddChatLine(string from, string message)
        {
                txtChatOutput.Text += from + ": " + message + "\n";
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_scaler != null)
                _scaler.TriggerRescale((sender as Slider).Value / 100);
        }
    }
        class CardScaler
        {
            private List<UserControl> _cards;
            private double _scale;


            public CardScaler(double initialScale)
            {
                Init(initialScale);
            }

            private void Init(double initialScale)
            {
                _scale = initialScale;
                _cards = new List<UserControl>();
            }

            public void Watch(UserControl c)
            {
                ResizeCard(c);
                _cards.Add(c);
            }
            public void UnWatch(UserControl c)
            {
                _cards.Remove(c);
            }

            public void TriggerRescale(double newScale)
            {
                _scale = newScale;
                foreach (var card in _cards)
                {
                    ResizeCard(card);
                }
            }

            private void ResizeCard(UserControl c)
            {
                var card = c as Card;
                if (card != null)
                {
                    var transform = card.GetTransformation(typeof (ScaleTransform)) as ScaleTransform;
                    if (transform == null)
                    {
                        transform = new ScaleTransform();
                        card.AddTransformation(transform);
                    }
                    transform.ScaleY = _scale;
                    transform.ScaleX = _scale;
                }else
                {
                    var t = new ScaleTransform(_scale, _scale);
                    c.RenderTransform = t;
                }
            }
        }
    }
