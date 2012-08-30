// File name: StackInitializeMessage.cs
// Creator: Jonatan Pålsson
// Solution: DeckOfCards
// Project: Networking
// Creation date: 2012-08-22-5:30 PM
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CardLogic;

namespace Networking.Messages
{
    [Serializable]
    public class StackInitializeMessage : BaseMessage
    {
        public StackObj NewStack;
    }
}
