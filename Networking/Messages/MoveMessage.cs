// File name: MoveMessage.cs
// Creator: Jonatan Pålsson
// Solution: DeckOfCards
// Project: Networking
// Creation date: 2012-08-22-5:28 PM
// 
using System;
using System.Windows;

namespace Networking.Messages
{
    [Serializable]
    public class MoveMessage : BaseMessage
    {
        public enum MoveType
        {
            Absolute,
            Relative
        }
        public Guid What;
        public Point Where;
        public String Comment;
        public MoveType Type;
        public override string ToString()
        {
            return "Move " + What + " to " + Where + ", Comment: " + Comment;
        }
    }
}
