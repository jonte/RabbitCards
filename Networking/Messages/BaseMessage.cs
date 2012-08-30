// File name: BaseMessage.cs
// Creator: Jonatan Pålsson
// Solution: DeckOfCards
// Project: Networking
// Creation date: 2012-08-22-5:28 PM
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Networking.Messages
{
    [Serializable]
    public class BaseMessage
    {
        public string OriginFriendlyName;
        public Guid OriginId;
    }
}
