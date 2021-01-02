using System;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace StargateBot
{
    class Program
    {
        
        //set the version number, so we can check which version is running on the server

        static void Main(string[] args)
                => new Program().MainAsync().GetAwaiter().GetResult();
        private DiscordSocketClient _client;
        private CommandService _commands;


        private IServiceProvider _services;

        public async Task MainAsync()
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                Bot.version = $"{Bot.version}.debug"; // Set versioning when in debug mode
                Global.debugmode = true;
            }

            Program.Debug("Oberoth debug mode active.");

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            Program.Debug("Creating new discord socket...");
            _client = new DiscordSocketClient(); //Create the discord client.

            Program.Debug("Attaching log process...");
            _client.Log += Log; // log important stuff
            _commands = new CommandService();
            
           
            string ver = Bot.version;

            await Log($"{Global.appname} v{ver}");
            //Console.WriteLine($"{Global.appname} v{ver}");
            Console.Title = $"{Global.appname} v{ver}";

            Program.Debug("Creating services...");
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();
            Program.Debug("Installing commands.");
            await InstallCommandsAsync(); // Install the command service
            Program.Debug("Token?");
            string token = Global.Token; // Our secret little token that is the source of all our power.
            Program.Debug("Log in to Discord");
            await _client.LoginAsync(TokenType.Bot, token); // We should login, shouldn't we?
            Program.Debug("Start the client");
            await _client.StartAsync(); //Start the client.
            Program.Debug("Set the game");
            await _client.SetGameAsync($"{Bot.name} v{Bot.version}"); // Set the running game to our current version.
            
            

            _client.UserJoined += async (s) =>
            {
                Program.Debug("A user has joined.");
                if (s.IsBot == false)
                {
                    Program.Debug("User is not a bot.");
                    Console.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy")} {DateTime.Now.ToString("HH:mm:ss")} New user has joined: {s.Username}"); // Log to the console that someone joined us.
                    var channel = s.Guild.GetTextChannel(Global.Channels.welcome);

                    Program.Debug("Getting random seed.");
                    Random rnd = new Random();
                    int rngrole = rnd.Next(1, 6);
                    
                    if (rngrole == 1)
                    {
                        var user = s;
                        var role = s.Guild.Roles.FirstOrDefault(x => x.Name == "Tau'ri");
                        Runtime.therole = "Tau'ri";
                        await (user as IGuildUser).AddRoleAsync(role);
                    } else if (rngrole == 2)
                    {
                        var user = s;
                        var role = s.Guild.Roles.FirstOrDefault(x => x.Name == "The Nox");
                        Runtime.therole = "The Nox";
                        await (user as IGuildUser).AddRoleAsync(role);
                    }
                    else if (rngrole == 3)
                    {
                        var user = s;
                        var role = s.Guild.Roles.FirstOrDefault(x => x.Name == "Tollan");
                        Runtime.therole = "Tollan";
                        await (user as IGuildUser).AddRoleAsync(role);
                    }
                    else if (rngrole == 4)
                    {
                        var user = s;
                        var role = s.Guild.Roles.FirstOrDefault(x => x.Name == "Wraith");
                        Runtime.therole = "Wraith";
                        await (user as IGuildUser).AddRoleAsync(role);
                    }
                    else if (rngrole == 5)
                    {
                        var user = s;
                        var role = s.Guild.Roles.FirstOrDefault(x => x.Name == "Furlings");
                        Runtime.therole = "Furlings";
                        await (user as IGuildUser).AddRoleAsync(role);
                    }
                    else if (rngrole == 6)
                    {
                        var user = s;
                        var role = s.Guild.Roles.FirstOrDefault(x => x.Name == "Asgard");
                        Runtime.therole = "Asgard";
                        await (user as IGuildUser).AddRoleAsync(role);
                    }
                    Program.Debug("Sending join message");
                    await channel.SendMessageAsync($"We are receiving a GDO transmission. It's {s.Mention}. Opening the iris.\n\nWelcome to our little corner of the ~~galaxy~~ internet, {s.Username}.\n\nWe would love to know a little about you. How long have you been a Stargate fan? What is your favourite episode? Who is your favourite character?\n\nI have assigned you the {Runtime.therole} role. If you wish to change your role, please use the !role command.\n\nIf you would like to participate in our drawing events, please send the command \"!drawing\".\n\nIf you would like to participate in our roleplaying events, please send the command \"!roleplay\".\n\nPlease be sure to check out #the-rules and enjoy your stay."); // Announce them to the world
                    Runtime.therole = "";
                }
                else
                {
                    Program.Debug("Bot detected.");
                    Console.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy")} {DateTime.Now.ToString("HH:mm:ss")} New bot user has joined: {s.Username}"); // Log to the console that someone joined us.
                }
                

            };

            _client.UserBanned += async (s, r) =>
            {
                var channel = r.GetTextChannel(Global.Channels.adminlog);
                await channel.SendMessageAsync($"Uh oh. {s.Username} has been banned.");
            };

            _client.UserUnbanned += async (s, r) =>
            {
                var channel = r.GetTextChannel(Global.Channels.adminlog);
                await channel.SendMessageAsync($"{s.Username} has been unbanned.");
            };

            _client.UserLeft += async (s) =>
            {
                Program.Debug("Oh noes! User has left!");
                if (s.IsBot == false)
                {
                    Program.Debug("User is not a bot. Sending message.");
                    Console.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy")} {DateTime.Now.ToString("HH:mm:ss")} User left: {s.Username}"); // Why'd they leave? :(
                    var channel = s.Guild.GetTextChannel(Global.Channels.welcome);
                    await channel.SendMessageAsync($"Chevron seven... Locked. Wormhole established.\n\n{s.Username} has left the server. I hope they return to Stargate Command in the future."); // Say good bye to our comrade.
                    //await _client.CurrentUser.SendMessageAsync($"Hey {s.Username}.\n\nI noticed you have left the Stargate Command server. \n\nIf there is something we can help make the experience better for you, please let @Myrddin#4392 or @AussieEevee#3149 know how we can help.\n\nThank you for being a part of our community, and we hope to see you again in the future.");
                }
                else
                {
                    Console.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy")} {DateTime.Now.ToString("HH:mm:ss")} Bot user left: {s.Username}"); // Why'd they leave? :(
                }

            };
            // Block this task until the program is closed.
            await Task.Delay(-1);
            
        }
        public async Task InstallCommandsAsync()
        {
            Program.Debug("Installing commands.");
            // Hook the MessageReceived Event into our Command Handler
            _client.MessageReceived += HandleCommandAsync;
            // Discover all of the commands in this assembly and load them.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(),_services);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            Program.Debug($"Message detected: {messageParam}");
            // Don't process the command if it was a System Message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;
            // Determine if the message is a command, based on if it starts with '!' or a mention prefix
            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))) return;
            Program.Debug($"Message is command.");
            // Create a Command Context
            var context = new SocketCommandContext(_client, message);
            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed successfully)
            Program.Debug($"Instructing system to execute command.");
            var result = await _commands.ExecuteAsync(context, argPos, _services);

            if (!result.IsSuccess)
            {
                Program.Debug($"Command failed.");
                Console.WriteLine($"Faulty command detected: {context.User.Username} sent \"{messageParam}\": {result.ErrorReason}");
            }
            //await context.Channel.SendMessageAsync(result.ErrorReason);
           
        }

        private Task Log (string msg)
        {
            string LogMessage = ($"{DateTime.Now.ToString("dd/MM/yyyy")} {msg.ToString()}");
            File.AppendAllText(Global.logfile, LogMessage + Environment.NewLine);
            //AdminLog(_client, msg);
            Console.WriteLine(LogMessage); // Log our logs to the console
            return Task.CompletedTask;
        }
         
        
        private Task Log(LogMessage msg)
        {
            string LogMessage = ($"{DateTime.Now.ToString("dd/MM/yyyy")} {msg.ToString()}");
            File.AppendAllText(Global.logfile, LogMessage + Environment.NewLine);
            //AdminLog(_client, msg.ToString());
            Console.WriteLine(LogMessage); // Log our logs to the console
            return Task.CompletedTask;
        }

        async void OnProcessExit(object sender, EventArgs e)
        {
            await Log("StargateBot exit command received. Closing program." + Environment.NewLine + Environment.NewLine);
        }

        private static void Debug(string msg)
        {
            if(Global.debugmode == true)
            {
                Console.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy")} Debug: " + msg);
            }
        }
    };

    




}

