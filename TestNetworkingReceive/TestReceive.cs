// File name: TestReceive.cs
// Creator: Jonatan Pålsson
// Solution: DeckOfCards
// Project: TestNetworkingReceive
// Creation date: 2012-08-20-4:54 PM
// 
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Networking;
using Networking.Messages;
using TestUtils;

namespace TestNetworkingReceive
{
    [TestClass]
    public class TestReceive
    {
        private const string Hostname = "192.168.1.68";
        private const int Port = 5672;
        private const string Username = "guest";
        private const string Password = "guest";

        private Receive _receiver;

        [TestInitialize]
        public void Setup()
        {
            SetupReceiver();
            SetupConnectionInReceiver();
        }

        public void SetupReceiver() { _receiver = new Receive(); }
        public void SetupConnectionInReceiver() { _receiver.SetupConsumer(Hostname, Port,Username,Password, "1.1.1"); }

        [TestCleanup]
        public void Cleanup()
        {
            _receiver = null;
        }

        [TestMethod]
        public void ReceiveViaCallback()
        {
            var locked = true;
            var locked2 = true;
            var message = "";
            _receiver.RegisterConsumer(obj =>
                                           {
                                               if (obj is ChatMessage)
                                               {
                                                   locked = false;
                                                   message = ((ChatMessage) obj).MessageText;
                                               }
                                               else if (obj is MoveMessage)
                                               {
                                                   locked2 = false;
                                               }
                                               return true;
                                           });
            while (locked || locked2) ;

        }
    }
}
