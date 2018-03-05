using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Discord.Addons.Interactive;
using Discord.Audio;
using TheNewBot.Services;
using System.Configuration;

namespace TheNewBot
{
    public class Program
    { 
        private CommandService commands;
        private DiscordSocketClient client;
        private IServiceProvider services;

        public static void Main(string[] args)
                    => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {

                    client = new DiscordSocketClient();

                    client.Log += Log;


            string token = ConfigurationManager.AppSettings["Token"].ToString();

            var services = ConfigureServices();
            //services.GetRequiredService<LogService>();
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync(services);

            await client.LoginAsync(TokenType.Bot, token);
                    await client.StartAsync();
                    //await InstallCommands();

            await Task.Delay(-1);
        }

                private Task Log(LogMessage msg)
                {
                    Console.WriteLine(msg.ToString());
                    return Task.CompletedTask;
                }
            

            public async Task InstallCommands()
            {
                // Hook the MessageReceived Event into Command Handler
                client.MessageReceived += MessageReceived;
                // Find and load all commands in this assembly
                await commands.AddModulesAsync(Assembly.GetEntryAssembly());
            }
        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                // Base
                .AddSingleton(client)
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<InteractiveService>()
                // Logging
                //.AddLogging()
                //.AddSingleton<LogService>()
                // Extra
                //.AddSingleton(_config)
                // Add additional services here...
                .BuildServiceProvider();
        }
        public async Task MessageReceived(SocketMessage messageParam)
            {

                // Don't process if a System Message
                var message = messageParam as SocketUserMessage;
                if (message == null) return;

            if (message.Content == "!ping")
            {
                await message.Channel.SendMessageAsync("Pong!");
            }

            // Create a number to track where the prefix ends and the commands begins
            int argPos = 0;

                // Determine if the message is a command, based on if it starts with '!' or a mention prefix. 
                if (!(message.HasCharPrefix('?', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos))) return;

                //Create a Command Context
                if(messageParam.Author != client.CurrentUser)
                {
                var context = new SocketCommandContext(client, message);

                // Execute the command. (result does not indicate a return value but an object stating
                // if the command executed successfully)
                var result = await commands.ExecuteAsync(context, argPos, services);

                if (!result.IsSuccess)
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                }

            }


    }
}
