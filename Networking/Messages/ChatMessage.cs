// File name: ChatMessage.cs
// Creator: Jonatan Pålsson
// Solution: DeckOfCards
// Project: Networking
// Creation date: 2012-08-22-5:28 PM
// 
using System;

namespace Networking.Messages
{
    [Serializable]
    public class ChatMessage : BaseMessage
    {
        public string MessageText;
    }
}
