// File name: Card.xaml.cs
// Creator: Jonatan Pålsson
// Solution: DeckOfCards
// Project: Cards
// Creation date: 2012-08-08-2:26 AM
// 
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using CardLogic;
using Microsoft.Expression.Interactivity.Layout;
using Microsoft.Expression.Media.Effects;

namespace Cards
{
    /// <summary>
    /// Interaction logic for Card.xaml
    /// </summary>
    public partial class Card
    {
        // The backing card
        private CardObj _cardObject;
        private bool _isDragging;
        private Point _clickPosition;
        private const int MessageThreshold = 10;
        private int _messageCounter;

        public Boolean Draggable = true;

        public event FinishedMoving FinishedMoving;
        public Card()
        {
            InitializeComponent();
        }

        public void AddTransformation(Transform tx)
        {
            var tg = RenderTransform as TransformGroup;
            if (tg == null)
            {
                tg = new TransformGroup();
                RenderTransform = tg;
            }
            // Don't add duplicates
            if (tg.Children.Any(child => child.GetType() == tx.GetType()))
            {
                return;
            }
            tg.Children.Add(tx);
            RenderTransform = tg;
        }

        public Transform GetTransformation(Type t)
        {
            var tg = RenderTransform as TransformGroup;
            if (tg == null) { return null; }
            return tg.Children.FirstOrDefault(child => child.GetType() == t);
        }

        /// <summary>
        /// Setter for the card object. Basically we check, in the setter, if we are
        /// setting a null value. If we are, then convert this card to an empty card.
        /// 
        /// If it is not a null value, register the proper events.
        /// </summary>
        public CardObj CardObject
        {
            get { return _cardObject; }
            set
            {
                // This means we have drawn the last card from the stack, and 
                //that we are trying to assign it to some card, probably the card representing the stack.
                if (value == null)
                {
                    _cardObject = null;
                    ShowCardEmpty();
                }
                else
                {
                    if (_cardObject != null)
                    {
                        _cardObject.CardBackChanged -= ShowCardBack;
                        _cardObject.CardChanged -= SetCardFace;
                        _cardObject.CardDestroyed -= CardDestroyed;
                    }
                    _cardObject = value;
                    SetCardFace(value.Suite, value.Value);
                    _cardObject.CardChanged += SetCardFace;
                    _cardObject.CardBackChanged += ShowCardBack;
                    _cardObject.CardDestroyed += CardDestroyed;
                }
            }
        }

        /// <summary>
        /// When this GUI-card is destroyed, find the parent of the card (currently only Canvas 
        /// parents are allowed, but this can easily be changed), and remove this element from
        /// that parent. This will have the effect of destroying the card visually.
        /// </summary>
        private void CardDestroyed()
        {
            DependencyObject parent = VisualTreeHelper.GetParent(this);
            if (parent is Canvas)
                ((Canvas)parent).Children.Remove(this);
            else
            {
                Console.Error.WriteLine("Parent is not a canvas!");
            }
        }

        /// <summary>
        /// Display the empty card placeholder
        /// </summary>
        private void ShowCardEmpty()
        {
                var cardPath = CardObj.GetCardEmptyPath();
                LoadCardGraphicFromXAML(cardPath);
        }


        /// <summary>
        /// Toggle showing card backs or showing card faces
        /// </summary>
        /// <param name="toggle"></param>
        private void ShowCardBack(bool toggle)
        {
            if (toggle)
            {
                var cardPath = CardObject.GetCardBackPath();
                LoadCardGraphicFromXAML(cardPath);
            }
            else
            {
                if (CardObject != null)
                {
                    SetCardFace(CardObject.Suite, CardObject.Value);
                }
                else
                {
                    ShowCardEmpty();
                }
            }
        }

        /// <summary>
        /// Set a card face
        /// </summary>
        /// <param name="cf"></param>
        /// <param name="value"></param>
        private void SetCardFace(CardSuite cf, int value)
        {
            var cardPath = CardObject.GetCardFacePath();
            LoadCardGraphicFromXAML(cardPath);
        }

        /// <summary>
        /// Load a card graphic from XAML, when loaded, store it in the canvas of the card (destroying
        /// any other child of the canvas)
        /// </summary>
        /// <param name="cardPath"></param>
        private void LoadCardGraphicFromXAML(string cardPath, int tries = 0)
        {
            Debug.Assert(cardPath != null, "Tried to load null card.. This is bad.");
            try
            {
                var s = new FileStream(cardPath, FileMode.Open);
                var card = (UIElement) XamlReader.Load(s);
                canvas.Children.Clear();
                canvas.Children.Add(card);
            }catch(IOException e) // The file is locked.. Try again
            {
                if (tries < 10000)
                    LoadCardGraphicFromXAML(cardPath, tries+1);
                else
                {
                    throw new IOException("Failed to load " + cardPath);
                }
            }
        }

        /// <summary>
        /// Toggle a visual select key (blue tint)
        /// </summary>
        /// <param name="activate"></param>
        private void ToggleSelect(bool activate)
        {
            var tone = new ColorToneEffect {LightColor = Colors.AliceBlue, DarkColor = Colors.CornflowerBlue};
            var child = canvas.Children[0];
            child.Effect = activate ? tone : null;

        }
        
        // Mouse action
        private void UserControlMouseUp(object sender, MouseButtonEventArgs e)
        { ToggleSelect(false); }

        // Mouse action
        private void UserControlMouseDown(object sender, MouseButtonEventArgs e)
        { ToggleSelect(true); }

        // Pretty-print card. Useful when adding card to lists or such
        public override string ToString()
        {
            if (CardObject != null)
            {
                return CardObject.Value + " of " + CardObject.Suite;
            }
            return "Empty card";
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (! Draggable) return;
            _isDragging = true;
            (sender as UserControl).CaptureMouse();
            var scale = GetTransformation(typeof (ScaleTransform)) as ScaleTransform;
            if (scale != null)
            _clickPosition = new Point((ActualWidth*scale.ScaleX)/2,(ActualHeight*scale.ScaleY)/2);
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (! Draggable) return;
            _isDragging = false;
            (sender as UserControl).ReleaseMouseCapture();
            if (FinishedMoving != null)
            {
                var transform = GetTransformation(typeof (TranslateTransform)) as TranslateTransform;
                if (transform == null)
                {
                    transform = new TranslateTransform();
                    AddTransformation(transform);
                }
                FinishedMoving(this, new Point(transform.X, transform.Y));
            }
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (! Draggable) return;
            {

                if (_isDragging)
                {
                    Point currentPosition = e.GetPosition(Parent as UIElement);


                    var transform = GetTransformation(typeof(TranslateTransform)) as TranslateTransform;
                    if (transform == null)
                    {
                        transform = new TranslateTransform();
                        AddTransformation(transform);
                    }

                    transform.X = currentPosition.X - _clickPosition.X;
                    transform.Y = currentPosition.Y - _clickPosition.Y;
                    if (_messageCounter++ >= MessageThreshold)
                    {
                        if (FinishedMoving != null)
                            FinishedMoving(this, new Point(transform.X, transform.Y));
                        _messageCounter = 0;
                    }
                }
            }
        }

        public void AbsoluteMove(double x, double y)
        {
            var transform = GetTransformation(typeof(TranslateTransform)) as TranslateTransform;
            if (transform == null)
            {
                transform = new TranslateTransform();
                AddTransformation(transform);
            }

            transform.X = x;
            transform.Y = y;
        }
    }

    public delegate void FinishedMoving(object sender, Point newPosition);
}
