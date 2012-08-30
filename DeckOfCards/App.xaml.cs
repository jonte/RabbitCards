// File name: App.xaml.cs
// Creator: Jonatan Pålsson
// Solution: DeckOfCards
// Project: DeckOfCards
// Creation date: 2012-08-19-7:23 PM
// 
using System;
using System.Windows;

namespace DeckOfCards
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        public static void Main(string[] param)
        {
            var mw = new MainWindow(param);
            mw.ShowDialog();
        }
    }
}
