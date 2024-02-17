using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using Ker_NelBot.Commands;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace Ker_NelBot
{
    public class Bot
    {
        public DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        public async Task RunAsync()
        {
            var json = string.Empty;
            
            using (var fs = File.OpenRead("config.json"))
                using (var sr = new StreamReader(fs, new UTF8Encoding(false))) 
                    json = await sr.ReadToEndAsync().ConfigureAwait(false);
            
            var configJSON = JsonConvert.DeserializeObject<ConfigJSON>(json);
            
            var logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

            var config = new DiscordConfiguration()
            {
                Token = configJSON.Token,
                TokenType = DSharpPlus.TokenType.Bot,
                AutoReconnect = true,
                LoggerFactory = new LoggerFactory().AddSerilog(logger),
                Intents = DiscordIntents.AllUnprivileged //| DiscordIntents.Contents,
            };
            
            Client = new DiscordClient(config);
            
            Client.Ready += OnClientReady;
            Client.ComponentInteractionCreated += ButtonPressResponse;
            
            Client.UseInteractivity(new InteractivityConfiguration
            {
                PollBehaviour = DSharpPlus.Interactivity.Enums.PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromMinutes(2)
            });
            
            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] {configJSON.Prefix},
                EnableMentionPrefix = true,
                EnableDms = false,
                DmHelp = false,
                IgnoreExtraArguments = true,
                UseDefaultCommandHandler = true,
                EnableDefaultHelp = false
            };
            
            Commands = Client.UseCommandsNext(commandsConfig);
            
            Commands.RegisterCommands<FunCommands>();
            Commands.RegisterCommands<ValCommands>();
            
            var list = new List<string>
            {
                "Fragment!",
                "Throwing charge.",
                "Grenade!",
                "Grenade out.",
                "They're in pieces.",
                "No chance.",
                "Flashbang.",
                "Flash!",
                "Shutting them down.",
                "Suppressing!",
                "Knife deployed.",
                "You are powerless!",
                "Emergency reset required.",
                "Requesting manual reset."
            };
            
            Random rnd = new Random();
            int statusNum = rnd.Next(0, 14);
            
            DiscordActivity status = new DiscordActivity("Valorant, " + list[statusNum], ActivityType.Playing);
            
            await Client.ConnectAsync(status);
            
            await Task.Delay(-1);
        }

        private async Task ButtonPressResponse(DiscordClient sender, ComponentInteractionCreateEventArgs e)
        {
            if (e.Interaction.Data.CustomId == "button1")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent("Button 1 pressed!"));
            } 
            else if (e.Interaction.Data.CustomId == "button2")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent("Button 2 pressed!"));
            }

            if (e.Interaction.Data.CustomId == "funButton")
            {
                string funCommandsList = "!8ball <question> -> **Ask a magic 8-ball a question**\n" +
                                         "!echo <message> -> **Repeats the message sent**\n" +
                                         "!factorial <number> -> **Calculate the factorial of a number**\n" +
                                         "!flipcoin -> **Flip one coin**\n" +
                                         "!flipcoin <amount> -> **Flip the designated amount of coins**\n" +
                                         "!joke -> **Creates a random joke response**\n" +
                                         "!meme -> **Gets a random meme**\n" +
                                         "!ping! -> **Pong!**\n" +
                                         "!poll <time> <emoji(s)> -> **Creates a poll of a certain amount of time with the specified emojis**\n" +
                                         "!randemj <@user> -> **Responds to the designated users latest message with a random emoji**\n" +
                                         "!reverse <message> -> **Reverses the message sent**\n" +
                                         "!roll <sides> -> **Creates a dice of n sides and gets what it lands on**\n" +
                                         "!rps -> **Rock, Paper, Scissors**\n" +
                                         "!translate <language> <message> -> **Translates the message from English to the designated language**\n" +
                                         "!trivia -> **Gets a random trivia question**\n";
                
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Fun Commands",
                    Description = funCommandsList,
                    Color = DiscordColor.Red
                };
                
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(embed));
            }
            else if (e.Interaction.Data.CustomId == "valButton")
            {
                string valCommandsList = "!agents -> **Gets a list of all the agents**\n" +
                                         "!maps -> **Gets a list of all the maps**\n" +
                                         "!roulette -> **Gets a random challenge to do in a Valorant round**\n" +
                                         "!roulette <amount> -> **Gets the specified amount of random challenges to do in a Valorant round**\n";

                var embed = new DiscordEmbedBuilder
                {
                    Title = "Valorant Commands",
                    Description = valCommandsList,
                    Color = DiscordColor.Red
                };
                
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(embed));
            }

            if (e.Interaction.Data.CustomId == "correctanswer")
            {
                var user = e.Interaction.User;
                await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent(user.Mention + " got the correct answer!"));
            }
            else if (e.Interaction.Data.CustomId == "answer1" || e.Interaction.Data.CustomId == "answer2" || e.Interaction.Data.CustomId == "answer3" || e.Interaction.Data.CustomId == "answer4")
            {
                var user = e.Interaction.User;
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(user.Mention + " got the wrong answer!"));
            }
        }

        private Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}