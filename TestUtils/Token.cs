// File name: Token.cs
// Creator: Jonatan Pålsson
// Solution: DeckOfCards
// Project: TestUtils
// Creation date: 2012-08-20-5:13 PM
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestUtils
{
    public class Token
    {
        static string _tempPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        public static string ConsumeTestToken(string key)
        {
            var filename = _tempPath + "/" + key + "_token";
            if (!System.IO.File.Exists(filename))
            {
                return "";
            }
            var val = System.IO.File.ReadAllText(filename);
            System.IO.File.Delete(filename);
            return val;
        }

        public static string WriteTestToken(string key)
        {
            var token = Guid.NewGuid().ToString();
            System.IO.File.WriteAllText(_tempPath + "/"+key+"_token", token);
            return token;
        }

    }
}
