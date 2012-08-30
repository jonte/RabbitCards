// File name: StackObj.cs
// Creator: Jonatan Pålsson
// Solution: DeckOfCards
// Project: CardLogic
// Creation date: 2012-08-08-11:56 AM
// 
using System;
using System.Collections.Generic;
using System.Linq;

namespace CardLogic
{
    /// <summary>
    /// Represents a stack of cards. Provides some convenience functionality such as shuffling and drawing.
    /// </summary>
    [Serializable]
    public class StackObj
    {
        // List of cards not yet drawn
        private List<CardObj> _undrawnCards = new List<CardObj>();

        // List of cards drawn, used for book-keeping. This is useful when wanting
        // to destroy all drawn cards
        private List<CardObj> _drawnCards = new List<CardObj>();

        // Fired when the stack is empty
        public event NoMoreCardsEvent NoMoreCards;

        // Top card
        public CardObj TopCard;

        /// <summary>
        /// Per defaul we will create 13 cards of each suite, and shuffle the deck
        /// </summary>
        public StackObj()
        {
            Populate(13,13,13,13);
            Shuffle();
        }

        private List<CardObj> UndrawnCards
        {
            get { return _undrawnCards; }
            set { _undrawnCards = value; }
        }

        private List<CardObj> DrawnCards
        {
            get { return _drawnCards; }
            set { _drawnCards = value; }
        }

        /// <summary>
        /// Adds a set number of cards for each suite starting from 1 going to N.
        /// </summary>
        /// <param name="numSpades"></param>
        /// <param name="numHearts"></param>
        /// <param name="numClubs"></param>
        /// <param name="numDiamonds"></param>
        private void Populate(int numSpades, int numHearts, int numClubs, int numDiamonds)
        {
            for (int i = 1; i <= numSpades; i++) { UndrawnCards.Add(new CardObj(CardSuite.Spades, i, "blue")); }
            for (int i = 1; i <= numClubs; i++) { UndrawnCards.Add(new CardObj(CardSuite.Clubs, i,"blue")); }
            for (int i = 1; i <= numDiamonds; i++) { UndrawnCards.Add(new CardObj(CardSuite.Diamonds, i,"blue")); }
            for (int i = 1; i <= numHearts; i++) { UndrawnCards.Add(new CardObj(CardSuite.Hearts, i,"blue")); }
        }

        /// <summary>
        /// Shuffle by greating a UUID (Microsoft calls them GUID) for each card, then simply sort
        /// the deck by the UUIDs (presumably their .ToString() property is used)
        /// </summary>
        private void Shuffle()
        {
            // Courtesy of Jeff Atwood: http://www.codinghorror.com/blog/2007/12/shuffling.html
            UndrawnCards = UndrawnCards.OrderBy(a => Guid.NewGuid()).ToList();
        }

        /// <summary>
        /// Draw a card, or draw null and fire a NoMoreCards event.
        /// </summary>
        /// <returns></returns>
        public CardObj Draw()
        {
            if (UndrawnCards.Count > 0)
            {
                var card = UndrawnCards[0];
                DrawnCards.Add(card);
                UndrawnCards.RemoveAt(0);
                TopCard = card;
                return card;
            }
            NoMoreCards();
            TopCard = null;
            return null;
        }

        /// <summary>
        /// Return a card from the drawn cards to the undrawn cards. It is added at the back
        /// </summary>
        /// <param name="co"></param>
        public void ReturnCard(CardObj co)
        {
            UndrawnCards.Insert(UndrawnCards.Count, co);
            DrawnCards.Remove(co);
        }

        /// <summary>
        /// Allows checking if there are more cards in the stack
        /// </summary>
        /// <returns></returns>
        public bool HasMoreCards()
        {
            return UndrawnCards.Count >= 0;
        }

        /// <summary>
        /// Destroy all cards in the stack
        /// </summary>
        public void Destroy()
        {
            foreach (var drawnCard in DrawnCards)
            {
                drawnCard.Destroy();
            }
        }

        public override string ToString()
        {
            return UndrawnCards.Aggregate("", (current, card) => current + (card + "\n")) +"==\n" 
                + DrawnCards.Aggregate("", (current,card) => current + (card + "\n"));
        }

        public StackObj GetSerializableCopy()
        {
            return new StackObj
                         {
                             TopCard = TopCard.GetSerializableCopy(),
                             UndrawnCards = UndrawnCards.Select(card => card.GetSerializableCopy()).ToList(),
                             DrawnCards = DrawnCards.Select(card => card.GetSerializableCopy()).ToList()
                         };
        }
    }

    public delegate void NoMoreCardsEvent();
}
