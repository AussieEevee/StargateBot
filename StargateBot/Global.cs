using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StargateBot
{
    class Bot
    {
        public static string version = $"0.8.6";
        public static string name = "Oberoth";
    }
    class Global
    {
        public static int Number { get; set; }
        public static string[] memes = Directory.GetFiles(@"Memes/");
        public static bool debugmode = false;
        
        
        public static string appname = "StargateBot";
        public static string logfile = @"sgbotlog.txt";

        //private static string token = File.ReadAllText(@"c:\important\sgtoken.txt", Encoding.UTF8); // Visual Studio keeps uploading Sneaky.cs... so we'll load the token from a file that Visual Studio can't see.
        private static string token = File.ReadAllText(@"sgtoken.txt", Encoding.UTF8); // Visual Studio keeps uploading Sneaky.cs... so we'll load the token from a file that Visual Studio can't see.
        public static string Token { get => token; } // Assign the token string to a public string so it can be accessed by the rest of the program.

        public static ulong GuildID = 385950750921981964;

        public class Channels
        {
            public static ulong adminlog = 395980764250112000;
            public static ulong welcome = 389788584284258306;
        }
        
    }

    class Runtime
    {
        public static string therole;
    }
}
  