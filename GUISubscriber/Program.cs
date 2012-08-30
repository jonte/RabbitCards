// File name: Program.cs
// Creator: Jonatan Pålsson
// Solution: DeckOfCards
// Project: GUISubscriber
// Creation date: 2012-08-21-4:59 PM
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GUISubscriber
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            

            Application.Run(new Form1());

        }
    }
}
