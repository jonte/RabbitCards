// File name: Stack1.xaml.cs
// Creator: Jonatan Pålsson
// Solution: DeckOfCards
// Project: Cards
// Creation date: 2012-08-16-8:19 PM
// 
using System.Windows.Interactivity;
using System.Windows.Media;
using CardLogic;

namespace Cards
{
    /// <summary>
    /// This is basically a card, but has some nice functionality for spawning new GUI cards
    /// </summary>
    public partial class Stack1
    {
        private StackObj _stack;
        private bool _showBack = true;
        public Stack1()
        {
            InitializeComponent();
            card1.Draggable = false;
        }

        public StackObj Stack
        {
            get { return _stack; }
        }

        public new Transform RenderTransform
        {
            get { return base.RenderTransform; }
            set { 
                base.RenderTransform = value;
                if (card1 != null)
                    card1.RenderTransform = value;
            }
        }

        /// <summary>
        /// Connect a logical stack to this GUI stack
        /// </summary>
        /// <param name="stack"></param>
        public void ConnectStack(StackObj stack)
        {
            _stack = stack;
            _stack.NoMoreCards += () => SetStackEmpty(true);
            if(_stack.TopCard == null)
                _stack.Draw();
            card1.CardObject = _stack.TopCard;
            card1.CardObject.ShowBack = true; // Show back per default
        }

        /// <summary>
        /// Toggle showing the back or face of the topmost card
        /// </summary>
        /// <param name="showBack"></param>
        public void SetShowBack(bool showBack)
        {
            if (card1.CardObject != null)
            {
                _showBack = showBack;
                card1.CardObject.ShowBack = showBack;
            }
        }

        /// <summary>
        /// Spawn a new card
        /// </summary>
        /// <returns></returns>
        public Card SpawnClonedCard()
        {
            var newCard = DestructiveDrawNewCard();
            if (newCard != null)
            {
                newCard.CardObject.ShowBack = false; // Show the face of the newly drawn card
                newCard.RenderTransform = RenderTransform;
                newCard.Width = Width;
                newCard.Height = Height;
                return newCard;
            }
            return null;
        }

        /// <summary>
        /// Draw a new card from the stack, "destroying" it in the stack, add it to a GUI Card
        /// object, and return it.
        /// </summary>
        /// <returns></returns>
        private Card DestructiveDrawNewCard()
        {
            var oldCard = new Card {CardObject = card1.CardObject};
            var newCardObj = _stack.Draw();
            if (newCardObj != null)
            {
                card1.CardObject = newCardObj;
                card1.CardObject.ShowBack = _showBack;
                return oldCard;
            }
            if (card1.CardObject != null)// give the last card!
            {
                card1.CardObject = null;
                return oldCard;
            }
            return null;
        }

        /// <summary>
        /// Check if the logical stack has more cards
        /// </summary>
        /// <returns></returns>
        public bool HasMoreCards()
        {
            return _stack.HasMoreCards();
        }

        public void SetStackEmpty(bool empty)
        {
            //MessageBox.Show("Empty!");
        }
    }
}
