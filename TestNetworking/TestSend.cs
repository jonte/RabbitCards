// File name: TestSend.cs
// Creator: Jonatan Pålsson
// Solution: DeckOfCards
// Project: TestNetworkingSend
// Creation date: 2012-08-20-4:24 PM
// 
using System;
using System.Drawing;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Networking;
using Networking.Messages;
using TestUtils;

namespace TestNetworking
{
    [TestClass]
    public class TestSend
    {
        private string hostname = "192.168.1.68";
        private Send sender;

        [TestInitialize]
        public void Setup()
        {
            SetupSender();
            SetupConnectionInSender();
        }

        public void SetupSender() { sender = new Send(); }
        public void SetupConnectionInSender() { sender.CreateConnection(hostname,5672 ,"guest","guest"); }

        [TestCleanup]
        public void Cleanup()
        {
            sender = null;
        }

        [TestMethod]
        public void CreateConnection()
        {
            Assert.AreEqual(sender.CreateConnection(hostname,5672 ,"guest","guest"), true);
        }

        [TestMethod]
        public void SendChatMessage()
        {
            var token = Token.WriteTestToken("stage2");
            var message = new ChatMessage(token);
            sender.SendSerializableObject(message, "1.1.1");
        }

        [TestMethod]
        public void SendMovementMessage()
        {
            var token = Token.WriteTestToken("stage3");
            var message = new MoveMessage{Comment = "Hello from " + token,What = Guid.NewGuid(), Where = new Point(100,100)};
            sender.SendSerializableObject(message, "1.1.1");
        }

    }
}
