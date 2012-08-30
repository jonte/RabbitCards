// File name: DrawMessage.cs
// Creator: Jonatan Pålsson
// Solution: DeckOfCards
// Project: Networking
// Creation date: 2012-08-22-6:40 PM
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Networking.Messages
{
    [Serializable]
    public class DrawMessage : BaseMessage
    {
        public string DrawFrom = "stack";
        public bool DrawAll = false;
    }
}
