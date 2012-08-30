// File name: CardObj.cs
// Creator: Jonatan Pålsson
// Solution: DeckOfCards
// Project: CardLogic
// Creation date: 2012-08-08-11:30 AM
// 
using System;
using System.IO;

namespace CardLogic
{
    /// <summary>
    /// CardObj is responsible for holding the value of a card (suite & rank),
    /// as well as providing functions for finding the graphics of the current card.
    /// 
    /// It is also used when finding the graphics for "empty" cards, and is the 
    /// publisher of events occurring when the card changes.
    /// </summary>
    [Serializable]
    public class CardObj
    {

        private readonly CardSuite _suite;
        private readonly int _value;

        private bool _showBack;
        private readonly String _backColor;

        public CardSuite Suite { get { return _suite; } }
        public int Value { get { return _value; } }

        public Guid Id;

        // Occurs when the suit or rank of the card changes (shouldn't really happen)
        public event CardChangedEvent CardChanged;

        // When the back is toggled
        public event CardBackChangedEvent CardBackChanged;

        // When the card is destroyed
        public event CardDestroyedEvent CardDestroyed;

        public CardObj(CardSuite suite, int value, string backColor)
        {
            Id = Guid.NewGuid();
            // Ensure the card rank is in range (aces are 1:s)
            if (value < 1 || value > 13)
            {
                throw new Exception("Card value is out of range");
            }
            _value = value;
            _suite = suite;
            _backColor = backColor;
        }

        // Allow toggling the back fo the card, signal an event when there are 
        // listers for this event.
        public bool ShowBack
        {
            get { return _showBack; }
            set { _showBack = value;
                if (CardBackChanged != null) CardBackChanged(value);
            }
        }


        /// <summary>
        /// Returns path to xaml-file suitable for loading
        /// </summary>
        /// <returns></returns>
        public String GetCardFacePath()
        {
            if (_value < 1 || _value > 13)
            {
                throw new Exception("Card value is out of range");
            }
            var txtSuite = "";
            switch (_suite)
            {
                case CardSuite.Hearts:
                    txtSuite = "H";
                    break;
                case CardSuite.Clubs:
                    txtSuite = "C";
                    break;
                case CardSuite.Spades:
                    txtSuite = "S";
                    break;
                case CardSuite.Diamonds:
                    txtSuite = "D";
                    break;
            }
            // TODO: Add all cards to a bank, if this can be shared between widgets
            var cardPath = "./graphics/" + Enum.GetName(typeof(CardSuite),_suite) + "/" + _value + txtSuite + ".xaml";
            if (!File.Exists(cardPath))
                throw new Exception("Card graphics not found: " + cardPath);
            return cardPath;
        }

        /// <summary>
        /// Load the back for this card. The back depends on which color has been set for the back
        /// </summary>
        /// <returns></returns>
        public String GetCardBackPath()
        {
            var cardPath = "./graphics/Backs/" + _backColor.ToLower() + ".xaml";
            if (!File.Exists(cardPath))
                throw new Exception("Card graphics not found: " + cardPath);
            return cardPath;

        }

        /// <summary>
        /// Pretty-printing support
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Enum.GetName(typeof(CardSuite), _suite) + " " + _value;
        }

        /// <summary>
        /// Signal for any listener (probably a GUI card) that the backing card should be
        /// destroyed. It will be destroyed when all references to the card are dropped, 
        /// and the CardObj is GC'd.
        /// </summary>
        public void Destroy()
        {
            CardDestroyed();
        }

        /// <summary>
        /// Load the graphics for the "empty" card, signifying that there is no rank or suite
        /// associated with this CardObj
        /// </summary>
        /// <returns></returns>
        public static string GetCardEmptyPath()
        {
            const string cardPath = "./graphics/Other/empty.xaml";
            if (!File.Exists(cardPath))
                throw new Exception("Card graphics not found: " + cardPath);
            return cardPath;
        }

        public CardObj GetSerializableCopy()
        {
            return new CardObj(_suite, _value, _backColor) {Id = Id};
        }
    }

    public delegate void CardDestroyedEvent();
    public delegate void CardBackChangedEvent(bool backVisible);
    public delegate void CardChangedEvent(CardSuite suite, int value);
}
