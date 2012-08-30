// File name: ConnectionFailedException.cs
// Creator: Jonatan Pålsson
// Solution: DeckOfCards
// Project: Networking
// Creation date: 2012-08-23-12:41 PM
// 
using System;

namespace Networking.Exceptions
{
    /// <summary>
    /// Fired when a connection fails for any reason
    /// </summary>
    public class ConnectionFailedException : Exception
    {
        /// <summary>
        /// Connection has failed
        /// </summary>
        /// <param name="message">Reason for failure</param>
        public ConnectionFailedException(string message) : base(message)
        {
        }
    }
}