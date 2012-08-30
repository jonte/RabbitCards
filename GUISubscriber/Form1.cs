// File name: Form1.cs
// Creator: Jonatan Pålsson
// Solution: DeckOfCards
// Project: GUISubscriber
// Creation date: 2012-08-21-4:59 PM
// 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Networking;
using Networking.Messages;

namespace GUISubscriber
{
    public partial class Form1 : Form
    {
        private Bitmap _drawArea;
        private int positionX;
        public Form1()
        {
            InitializeComponent();

            _drawArea = new Bitmap(pictureBox1.Width,pictureBox1.Height);
            pictureBox1.Image = _drawArea;


            var receiver111 = new Receive();
            receiver111.SetupConsumer("192.168.1.68", 5672, "guest","guest", "1.1.1");
            receiver111.RegisterConsumer(obj =>
                                             {
                                                 DrawCircle();
                                                 return true;
                                             });


            var receiver222 = new Receive();
            receiver222.SetupConsumer("192.168.1.68", 5672, "guest","guest", "2.2.2");
            receiver222.RegisterConsumer(obj =>
                                             {
                                                 positionX += 100;
                                                 return true;
                                             });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var sendObj = new Send();
            var msg = new MoveMessage {Comment = "Message", What = Guid.NewGuid(), Where = new Point(100,100)};
            sendObj.CreateConnection("192.168.1.68", 5672, "guest", "guest");
            sendObj.SendSerializableObject(msg, "1.1.1");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var sendObj = new Send();
            var msg = new MoveMessage {Comment = "Message", What = Guid.NewGuid(), Where = new Point(100,100)};
            sendObj.CreateConnection("192.168.1.68", 5672, "guest", "guest");
            sendObj.SendSerializableObject(msg, "2.2.2");

        }

        private void DrawCircle()
        {
            MethodInvoker updateIt = delegate
                {
                    Graphics g;
                    g = Graphics.FromImage(_drawArea);
                    g.DrawEllipse(new Pen(Brushes.Black), positionX, 100, 30, 30);
                    g.Dispose();
                    Invalidate();
                    Update();
                    Refresh();
                };
            this.BeginInvoke(updateIt);
        }
    }
}
