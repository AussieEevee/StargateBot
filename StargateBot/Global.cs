using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StargateBot
{
    class Global
    {
        public static int Number { get; set; }
        public static string[] memes = Directory.GetFiles(@"Memes\");
        public static double version = 0.0001;
        public static string appname = "StargateBot";

        private static string text = File.ReadAllText(@"c:\important\sgtoken.txt", Encoding.UTF8); // Visual Studio keeps uploading Sneaky.cs... so we'll load the token from a file that Visual Studio can't see.
        private static string token = text; // Assign the text string to the token string. Why didn't we just directly assign the contents? Laziness.

        public static string Token { get => token; } // Assign the token string to a public string so it can be accessed by the rest of the program.
    }
}
  