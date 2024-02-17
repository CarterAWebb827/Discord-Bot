using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Ker_NelBot.Handlers.Dialogue;
using Ker_NelBot.Handlers.Dialogue.Steps;
using Newtonsoft.Json;

namespace Ker_NelBot.Commands
{
    using CommandContext = DSharpPlus.CommandsNext.CommandContext;

    public class FunCommands : BaseCommandModule
    {
        [Command("Ping!")]
        [Description("Ping! Pong!")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Pong!").ConfigureAwait(false);
        }

        [Command("Echo")]
        [Description("Repeats a message.")]
        public async Task Echo(CommandContext ctx, [RemainingText] string message)
        {
            await ctx.Channel.SendMessageAsync(message).ConfigureAwait(false);
        }

        [Command("Reverse")]
        [Description("Reverses a message.")]
        public async Task Reverse(CommandContext ctx, [RemainingText] string message)
        {
            char[] charArray = message.ToCharArray();
            Array.Reverse(charArray);
            await ctx.Channel.SendMessageAsync(new string(charArray)).ConfigureAwait(false);
        }

        [Command("joke")]
        [Description("Responds to a message with a joke.")]
        public async Task Joke(CommandContext ctx)
        {
            var client = new HttpClient();
            var response = await client
                .GetAsync("https://v2.jokeapi.dev/joke/Any?blacklistFlags=religious,political,racist,sexist")
                .ConfigureAwait(false);
            Joke randJoke;

            using (var httpClient = new HttpClient())
            {
                var json = await httpClient
                    .GetStringAsync("https://v2.jokeapi.dev/joke/Any?blacklistFlags=religious,political,racist,sexist")
                    .ConfigureAwait(false);
                randJoke = JsonConvert.DeserializeObject<Joke>(json);
            }

            if (randJoke.type == "single")
            {
                await ctx.Channel.SendMessageAsync(randJoke.joke).ConfigureAwait(false);
            }
            else
            {
                await ctx.Channel.SendMessageAsync(randJoke.setup).ConfigureAwait(false);
                await Task.Delay(3000).ConfigureAwait(false);
                await ctx.Channel.SendMessageAsync(randJoke.delivery).ConfigureAwait(false);
            }
        }

        [Command("randemj")]
        [Description("Responds to a message with an random emoji.")]
        public async Task RespondEMJ(CommandContext ctx, DiscordUser user)
        {
            var emojiList = NeoSmart.Unicode.Emoji.All;
            var rand = new Random();
            var randEmoji = emojiList.ElementAt(rand.Next(emojiList.Count()));

            var guild = await ctx.Client.GetGuildAsync(ctx.Guild.Id).ConfigureAwait(false);
            var member = await guild.GetMemberAsync(user.Id).ConfigureAwait(false);

            var latestMsgs = await ctx.Channel.GetMessagesAsync(10).ConfigureAwait(false);
            var latestMsg = latestMsgs.FirstOrDefault(x => x.Author.Id == user.Id);

            if (latestMsg != null)
            {
                await latestMsg.RespondAsync(randEmoji.ToString()).ConfigureAwait(false);
            }
            else
            {
                await ctx.Channel.SendMessageAsync($"{member.Mention} {randEmoji}").ConfigureAwait(false);
            }
        }

        [Command("Poll")]
        [Description("Creates a poll.")]
        public async Task Poll(CommandContext ctx,
            [Description("In the notation of TimeIndentifier. \nEx: 10s = 10 seconds")]
            TimeSpan duration,
            params DiscordEmoji[] emojiOptions)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var options = emojiOptions.Select(x => x.ToString());

            var pollEmbed = new DiscordEmbedBuilder
            {
                Title = "Poll",
                Description = string.Join(" ", options),
                Color = DiscordColor.Turquoise,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = ctx.Client.CurrentUser.AvatarUrl
                }
            };

            var pollMessage = await ctx.Channel.SendMessageAsync(embed: pollEmbed).ConfigureAwait(false);

            foreach (var option in emojiOptions)
            {
                await pollMessage.CreateReactionAsync(option).ConfigureAwait(false);
            }

            var result = await interactivity.CollectReactionsAsync(pollMessage, duration).ConfigureAwait(false);
            var distinctResult = result.Distinct();

            var strResult = distinctResult.Select(x => $"{x.Emoji}: {x.Total}");

            await ctx.Channel.SendMessageAsync(string.Join("\n", strResult)).ConfigureAwait(false);
        }

        [Command("Dialogue")]
        [Description("Starts a dialogue.")]
        public async Task Dialogue(CommandContext ctx)
        {
            var inputStep = new TextStep("Enter something interesting.", null);
            var intStep = new IntStep("Enter a number.", null);

            string input = string.Empty;
            int value = 0;

            inputStep.OnValidResult += (result) => input = result;
            intStep.OnValidResult += (result) => value = result;

            var inputDialogueHandler = new DialogueHandler(
                ctx.Client,
                ctx.Channel,
                ctx.User,
                inputStep);

            bool succeeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            if (!succeeded)
            {
                return;
            }

            await ctx.Channel.SendMessageAsync($"Your input: {input}").ConfigureAwait(false);

            await ctx.Channel.SendMessageAsync($"Your number: {value}").ConfigureAwait(false);
        }

        [Command("EmjDialogue")]
        [Description("Starts a dialogue with emojis.")]
        public async Task EmjDialogue(CommandContext ctx)
        {
            var yesStep = new TextStep("You said yes", null);
            var noStep = new TextStep("You said no", null);

            var emojiStep = new ReactionStep("Yes or No?", new Dictionary<DiscordEmoji, ReactionStepData>
            {
                {
                    DiscordEmoji.FromName(ctx.Client, ":thumbsup:"),
                    new ReactionStepData { Content = "Yes", NextStep = yesStep }
                },
                {
                    DiscordEmoji.FromName(ctx.Client, ":thumbsdown:"),
                    new ReactionStepData { Content = "No", NextStep = noStep }
                }
            });

            var inputDialogueHandler = new DialogueHandler(
                ctx.Client,
                ctx.Channel,
                ctx.User,
                emojiStep);

            bool succeeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            if (!succeeded)
            {
                return;
            }
        }

        [Command("meme")]
        [Description("Sends a random meme.")]
        public async Task Meme(CommandContext ctx)
        {
            var client = new HttpClient();
            var response = await client.GetAsync("https://meme-api.com/gimme").ConfigureAwait(false);
            var data = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false));

            var memeEmbed = new DiscordEmbedBuilder
            {
                Title = data.title,
                ImageUrl = data.url,
                Url = data.postLink,
                Color = DiscordColor.Turquoise,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"r/{data.subreddit}"
                }
            };

            await ctx.Channel.SendMessageAsync(embed: memeEmbed).ConfigureAwait(false);
        }

        [Command("trivia")]
        [Description("Provides a trivia question.")]
        public async Task Trivia(CommandContext ctx)
        {
            var response = await new HttpClient().GetAsync("https://opentdb.com/api.php?amount=1")
                .ConfigureAwait(false);
            var triviaResponse =
                JsonConvert.DeserializeObject<TriviaResponse>(await response.Content.ReadAsStringAsync()
                    .ConfigureAwait(false));
            var triviaQuestion = triviaResponse.results.First();

            var decodedQuestion = System.Net.WebUtility.HtmlDecode(triviaQuestion.question);
            var decodedCorrectAnswer = System.Net.WebUtility.HtmlDecode(triviaQuestion.correct_answer);
            var decodedIncorrectAnswers = triviaQuestion.incorrect_answers
                .Select(x => System.Net.WebUtility.HtmlDecode(x)).ToList();

            var shuffledAnswers = decodedIncorrectAnswers.Append(decodedCorrectAnswer).OrderBy(x => Guid.NewGuid())
                .ToList();
            var correctAnswerIndex = shuffledAnswers.IndexOf(decodedCorrectAnswer);

            var triviaEmbed = new DiscordEmbedBuilder
            {
                Title = "Trivia",
                Description = decodedQuestion,
                Color = DiscordColor.Turquoise,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Category: {triviaQuestion.category} | Difficulty: {triviaQuestion.difficulty}"
                }
            };

            DiscordButtonComponent answer1;
            DiscordButtonComponent answer2;
            DiscordButtonComponent answer3;
            DiscordButtonComponent answer4;

            if (correctAnswerIndex == 0)
            {
                answer1 = new DiscordButtonComponent(ButtonStyle.Primary, "correctanswer", shuffledAnswers[0]);
            }
            else
            {
                answer1 = new DiscordButtonComponent(ButtonStyle.Primary, "answer1", shuffledAnswers[0]);
            }

            if (correctAnswerIndex == 1)
            {
                answer2 = new DiscordButtonComponent(ButtonStyle.Primary, "correctanswer", shuffledAnswers[1]);
            }
            else
            {
                answer2 = new DiscordButtonComponent(ButtonStyle.Primary, "answer2", shuffledAnswers[1]);
            }

            if (correctAnswerIndex == 2)
            {
                answer3 = new DiscordButtonComponent(ButtonStyle.Primary, "correctanswer", shuffledAnswers[2]);
            }
            else
            {
                answer3 = new DiscordButtonComponent(ButtonStyle.Primary, "answer3", shuffledAnswers[2]);
            }

            if (correctAnswerIndex == 3)
            {
                answer4 = new DiscordButtonComponent(ButtonStyle.Primary, "correctanswer", shuffledAnswers[3]);
            }
            else
            {
                answer4 = new DiscordButtonComponent(ButtonStyle.Primary, "answer4", shuffledAnswers[3]);
            }

            var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle("Trivia")
                    .WithDescription(decodedQuestion)
                    .WithColor(DiscordColor.Turquoise)
                    .WithFooter($"Category: {triviaQuestion.category} | Difficulty: {triviaQuestion.difficulty}"))
                .AddComponents(answer1, answer2, answer3, answer4);

            await ctx.Channel.SendMessageAsync(message).ConfigureAwait(false);
        }

        [Command("button")]
        [Description("Starts a dialogue with buttons.")]
        public async Task Button(CommandContext ctx)
        {
            DiscordButtonComponent button1 = new DiscordButtonComponent(ButtonStyle.Primary, "button1", "Button 1");
            DiscordButtonComponent button2 = new DiscordButtonComponent(ButtonStyle.Primary, "button2", "Button 2");

            var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle("Button Test")
                    .WithDescription("This is a button test")
                    .WithColor(DiscordColor.Turquoise)
                    .WithThumbnail(ctx.Client.CurrentUser.AvatarUrl))
                .AddComponents(button1)
                .AddComponents(button2);

            await ctx.Channel.SendMessageAsync(message).ConfigureAwait(false);
        }

        [Command("help")]
        [Description("Shows the help menu.")]
        public async Task HelpCommand(CommandContext ctx)
        {
            var funButton = new DiscordButtonComponent(ButtonStyle.Success, "funButton", "Fun Commands");
            var valButton = new DiscordButtonComponent(ButtonStyle.Success, "valButton", "Valorant Commands");

            var helpMsg = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle("Help Menu")
                    .WithDescription(
                        "Please choose a button for information on the command type.\n(Fun Commands or Valorant Commands)")
                    .WithColor(DiscordColor.Red)
                    .WithThumbnail(ctx.Client.CurrentUser.AvatarUrl))
                .AddComponents(funButton, valButton);

            await ctx.Channel.SendMessageAsync(helpMsg).ConfigureAwait(false);
        }

        [Command("translate")]
        [Description("Translates a message from english to the designated language.")]
        public async Task Translate(CommandContext ctx,
            [Description("The language you would like to translate to\nI.E: Spanish, French, Japanese, etc.")]
            string lang,
            [Description("The message that you would like to translate.")] [RemainingText]
            string message)
        {
            var client = new HttpClient();
            var response = await client
                .GetAsync($"https://api.mymemory.translated.net/get?q={message}&langpair=en|{lang}")
                .ConfigureAwait(false);
            var data = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false));

            var translatedEmbed = new DiscordEmbedBuilder
            {
                Title = "Translation",
                Description = data.responseData.translatedText,
                Color = DiscordColor.Turquoise,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = ctx.Client.CurrentUser.AvatarUrl
                }
            };

            await ctx.Channel.SendMessageAsync(embed: translatedEmbed).ConfigureAwait(false);
        }

        [Command("8ball")]
        [Description("Ask the magic 8ball a question.")]
        public async Task EightBall(CommandContext ctx,
            [Description("The question you would like to ask the magic 8ball.")] [RemainingText]
            string question)
        {
            var answers = new List<string>
            {
                "It is certain.",
                "It is decidedly so.",
                "Without a doubt.",
                "Yes - definitely.",
                "You may rely on it.",
                "As I see it, yes.",
                "Most likely.",
                "Outlook good.",
                "Signs point to yes.",
                "Yes.",
                "Reply hazy, try again.",
                "Ask again later.",
                "Better not tell you now...",
                "Cannot predict now.",
                "Concentrate and ask again.",
                "Don't count on it.",
                "My reply is no.",
                "My sources say no...",
                "Outlook not so good...",
                "Very doubtful."
            };

            var random = new Random();
            var answer = answers[random.Next(answers.Count)];

            var eightBallEmbed = new DiscordEmbedBuilder
            {
                Title = "Magic 8Ball",
                Description = $"**Question:** {question}\n**Answer:** {answer}",
                Color = DiscordColor.Turquoise,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = ctx.Client.CurrentUser.AvatarUrl
                }
            };

            await ctx.Channel.SendMessageAsync(embed: eightBallEmbed).ConfigureAwait(false);
        }

        [Command("flipcoin")]
        [Description("Flips the designated amount of coins and shares the results.")]
        public async Task FlipCoin(CommandContext ctx,
            [Description("The amount of coins you would like to flip.")]
            int amount)
        {
            var random = new Random();
            var heads = 0;
            var tails = 0;

            for (var i = 0; i < amount; i++)
            {
                var coin = random.Next(2);
                if (coin == 0)
                {
                    heads++;
                }
                else
                {
                    tails++;
                }
            }

            var coinEmbed = new DiscordEmbedBuilder
            {
                Title = "Coin Flip",
                Description = $"Heads: {heads}\nTails: {tails}",
                Color = DiscordColor.Turquoise,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = ctx.Client.CurrentUser.AvatarUrl
                }
            };

            await ctx.Channel.SendMessageAsync(embed: coinEmbed).ConfigureAwait(false);
        }

        [Command("flipcoin")]
        [Description("Flips a coin.")]
        public async Task FlipCoin(CommandContext ctx)
        {
            var random = new Random();
            var coin = random.Next(2);

            var coinEmbed = new DiscordEmbedBuilder
            {
                Title = "Coin Flip",
                Description = coin == 0 ? "Heads" : "Tails",
                Color = DiscordColor.Turquoise,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = ctx.Client.CurrentUser.AvatarUrl
                }
            };

            await ctx.Channel.SendMessageAsync(embed: coinEmbed).ConfigureAwait(false);
        }

        [Command("roll")]
        [Description("Rolls a dice of the designated amount of sides.")]
        public async Task Roll(CommandContext ctx,
            [Description("The amount of sides the dice has.")]
            int sides)
        {
            if (sides < 1)
            {
                await ctx.Channel.SendMessageAsync("The dice must have at least 1 side.").ConfigureAwait(false);
                return;
            }


            var random = new Random();
            var roll = random.Next(1, sides + 1);

            var rollEmbed = new DiscordEmbedBuilder
            {
                Title = "Dice Roll",
                Description = $"You rolled a {roll}!",
                Color = DiscordColor.Turquoise,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = ctx.Client.CurrentUser.AvatarUrl
                }
            };

            await ctx.Channel.SendMessageAsync(embed: rollEmbed).ConfigureAwait(false);
        }

        [Command("rps")]
        [Description("Starts a game of rock, paper, scissors.")]
        public async Task Rps(CommandContext ctx)
        {
            if (ctx.User.Id == 570102082133032981)
            {
                var rpsEmbed = new DiscordEmbedBuilder
                {
                    Title = "Rock, Paper, Scissors",
                    Description = "Choose your weapon!",
                    Color = DiscordColor.Turquoise,
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                    {
                        Url = ctx.Client.CurrentUser.AvatarUrl
                    }
                };

                var rpsMessage = await ctx.Channel.SendMessageAsync(embed: rpsEmbed).ConfigureAwait(false);

                await rpsMessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":rock:")).ConfigureAwait(false);
                await rpsMessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":newspaper:"))
                    .ConfigureAwait(false);
                await rpsMessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":scissors:"))
                    .ConfigureAwait(false);

                var interactivity = ctx.Client.GetInteractivity();

                var reactionResult = await interactivity.WaitForReactionAsync(
                    x => x.Message == rpsMessage &&
                         x.User == ctx.User &&
                         (x.Emoji == DiscordEmoji.FromName(ctx.Client, ":rock:") ||
                          x.Emoji == DiscordEmoji.FromName(ctx.Client, ":newspaper:") ||
                          x.Emoji == DiscordEmoji.FromName(ctx.Client, ":scissors:"))).ConfigureAwait(false);

                if (reactionResult.Result.Emoji == DiscordEmoji.FromName(ctx.Client, ":rock:"))
                {
                    await ctx.Channel.SendMessageAsync(":scissors:").ConfigureAwait(false);
                    await ctx.Channel.SendMessageAsync("I chose scissors! You win!").ConfigureAwait(false);
                }
                else if (reactionResult.Result.Emoji == DiscordEmoji.FromName(ctx.Client, ":newspaper:"))
                {
                    await ctx.Channel.SendMessageAsync(":rock:").ConfigureAwait(false);
                    await ctx.Channel.SendMessageAsync("I chose rock! You win!").ConfigureAwait(false);
                }
                else if (reactionResult.Result.Emoji == DiscordEmoji.FromName(ctx.Client, ":scissors:"))
                {
                    await ctx.Channel.SendMessageAsync(":newspaper:").ConfigureAwait(false);
                    await ctx.Channel.SendMessageAsync("I chose paper! You win!").ConfigureAwait(false);
                }
            }
            else
            {
                var rpsEmbed = new DiscordEmbedBuilder
                {
                    Title = "Rock, Paper, Scissors",
                    Description = "Choose your weapon!",
                    Color = DiscordColor.Turquoise,
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                    {
                        Url = ctx.Client.CurrentUser.AvatarUrl
                    }
                };

                var rpsMessage = await ctx.Channel.SendMessageAsync(embed: rpsEmbed).ConfigureAwait(false);

                await rpsMessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":rock:")).ConfigureAwait(false);
                await rpsMessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":newspaper:"))
                    .ConfigureAwait(false);
                await rpsMessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":scissors:"))
                    .ConfigureAwait(false);

                var interactivity = ctx.Client.GetInteractivity();

                var reactionResult = await interactivity.WaitForReactionAsync(
                    x => x.Message == rpsMessage &&
                         x.User == ctx.User &&
                         (x.Emoji == DiscordEmoji.FromName(ctx.Client, ":rock:") ||
                          x.Emoji == DiscordEmoji.FromName(ctx.Client, ":newspaper:") ||
                          x.Emoji == DiscordEmoji.FromName(ctx.Client, ":scissors:"))).ConfigureAwait(false);

                var random = new Random();
                var rps = random.Next(2);

                if (reactionResult.Result.Emoji == DiscordEmoji.FromName(ctx.Client, ":rock:"))
                {
                    if (rps == 0)
                    {
                        await ctx.Channel.SendMessageAsync(":rock:").ConfigureAwait(false);
                        await ctx.Channel.SendMessageAsync("I chose rock! It's a tie!").ConfigureAwait(false);
                    }
                    else if (rps == 1)
                    {
                        await ctx.Channel.SendMessageAsync(":newspaper:").ConfigureAwait(false);
                        await ctx.Channel.SendMessageAsync("I chose paper! You lose!").ConfigureAwait(false);
                    }
                    else if (rps == 2)
                    {
                        await ctx.Channel.SendMessageAsync(":scissors:").ConfigureAwait(false);
                        await ctx.Channel.SendMessageAsync("I chose scissors! You win!").ConfigureAwait(false);
                    }
                }
                else if (reactionResult.Result.Emoji == DiscordEmoji.FromName(ctx.Client, ":newspaper:"))
                {
                    if (rps == 0)
                    {
                        await ctx.Channel.SendMessageAsync(":rock:").ConfigureAwait(false);
                        await ctx.Channel.SendMessageAsync("I chose rock! You win!").ConfigureAwait(false);
                    }
                    else if (rps == 1)
                    {
                        await ctx.Channel.SendMessageAsync(":newspaper:").ConfigureAwait(false);
                        await ctx.Channel.SendMessageAsync("I chose paper! It's a tie!").ConfigureAwait(false);
                    }
                    else if (rps == 2)
                    {
                        await ctx.Channel.SendMessageAsync(":scissors:").ConfigureAwait(false);
                        await ctx.Channel.SendMessageAsync("I chose scissors! You lose!").ConfigureAwait(false);
                    }
                }
                else if (reactionResult.Result.Emoji == DiscordEmoji.FromName(ctx.Client, ":scissors:"))
                {
                    if (rps == 0)
                    {
                        await ctx.Channel.SendMessageAsync(":rock:").ConfigureAwait(false);
                        await ctx.Channel.SendMessageAsync("I chose rock! You lose!").ConfigureAwait(false);
                    }
                    else if (rps == 1)
                    {
                        await ctx.Channel.SendMessageAsync(":newspaper:").ConfigureAwait(false);
                        await ctx.Channel.SendMessageAsync("I chose paper! You win!").ConfigureAwait(false);
                    }
                    else if (rps == 2)
                    {
                        await ctx.Channel.SendMessageAsync(":scissors:").ConfigureAwait(false);
                        await ctx.Channel.SendMessageAsync("I chose scissors! It's a tie!").ConfigureAwait(false);
                    }
                }
            }
        }

        [Command("factorial")]
        [Description("Calculates the factorial of a number.")]
        public async Task Factorial(CommandContext ctx,
            [Description("The number to calculate the factorial of.")]
            int number)
        {
            BigInteger factorial = 1;

            for (var i = 1; i <= number; i++)
            {
                factorial *= (BigInteger)i;
            }

            await ctx.Channel.SendMessageAsync($"{number}! = {factorial}").ConfigureAwait(false);
        }

        /*[Command("tictactoe")]
        [Description("Play a game of Tic Tac Toe with another user.")]
        public async Task TicTacToe(CommandContext ctx,
            [Description("The user to play against.")]
            DiscordMember opponent)
        {
            var tttEmbed = new DiscordEmbedBuilder
            {
                Title = "Tic Tac Toe",
                Description = $"{ctx.Member.Mention} vs. {opponent.Mention}",
                Color = DiscordColor.Turquoise,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = ctx.Client.CurrentUser.AvatarUrl
                }
            };

            var tttMessage = await ctx.Channel.SendMessageAsync(embed: tttEmbed).ConfigureAwait(false);

            await tttMessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":one:")).ConfigureAwait(false);
            await tttMessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":two:")).ConfigureAwait(false);
            await tttMessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":three:")).ConfigureAwait(false);
            await tttMessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":four:")).ConfigureAwait(false);
            await tttMessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":five:")).ConfigureAwait(false);
            await tttMessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":six:")).ConfigureAwait(false);
            await tttMessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":seven:")).ConfigureAwait(false);
            await tttMessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":eight:")).ConfigureAwait(false);
            await tttMessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":nine:")).ConfigureAwait(false);

            var interactivity = ctx.Client.GetInteractivity();

            var reactionResult = await interactivity.WaitForReactionAsync(
                    x => x.Message == tttMessage &&
                         x.User == ctx.User &&
                         (x.Emoji == DiscordEmoji.FromName(ctx.Client, ":one:") ||
                          x.Emoji == DiscordEmoji.FromName(ctx.Client, ":two:") ||
                          x.Emoji == DiscordEmoji.FromName(ctx.Client, ":three:") ||
                          x.Emoji == DiscordEmoji.FromName(ctx.Client, ":four:") ||
                          x.Emoji == DiscordEmoji.FromName(ctx.Client, ":five:") ||
                          x.Emoji == DiscordEmoji.FromName(ctx.Client, ":six:") ||
                          x.Emoji == DiscordEmoji.FromName(ctx.Client, ":seven:") ||
                          x.Emoji == DiscordEmoji.FromName(ctx.Client, ":eight:") ||
                          x.Emoji == DiscordEmoji.FromName(ctx.Client, ":nine:")), TimeSpan.FromSeconds(30))
                .ConfigureAwait(false);

            if (!reactionResult.TimedOut)
            {
                await ctx.Channel.SendMessageAsync("You didn't respond in time!").ConfigureAwait(false);
            }

            await tttMessage.DeleteAsync().ConfigureAwait(false);

            await ctx.Channel.SendMessageAsync($"{ctx.Member.Mention} vs. {opponent.Mention}").ConfigureAwait(false);

            await ctx.Channel.SendMessageAsync("1️⃣ 2️⃣ 3️⃣").ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync("4️⃣ 5️⃣ 6️⃣").ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync("7️⃣ 8️⃣ 9️⃣").ConfigureAwait(false);

            var tttBoard = new string[3, 3];

            var tttBoardMessage = await ctx.Channel.SendMessageAsync("```" + tttBoard[0, 0] + " " + tttBoard[0, 1] +
                                                                     " " + tttBoard[0, 2] + "\n" + tttBoard[1, 0] +
                                                                     " " + tttBoard[1, 1] + " " + tttBoard[1, 2] +
                                                                     "\n" + tttBoard[2, 0] + " " + tttBoard[2, 1] +
                                                                     " " + tttBoard[2, 2] + "```")
                .ConfigureAwait(false);

            var tttBoardInteractivity = ctx.Client.GetInteractivity();
        }*/
        
        /*[Command("hangman")]
        [Description("Play a game of Hangman.")]
        public async Task Hangman(CommandContext ctx)
        {
            var client = new HttpClient();
            var word = await client.GetStringAsync("https://random-word-api.herokuapp.com/word?lang=es").ConfigureAwait(false);
            
            var hangmanWord = new string[word.Length];
            
            for (var i = 0; i < word.Length; i++)
            {
                hangmanWord[i] = "_";
            }
            
            var hangmanEmbed = new DiscordEmbedBuilder
            {
                Title = "Hangman",
                Description = $"{ctx.Member.Mention}",
                Color = DiscordColor.Turquoise,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = ctx.Client.CurrentUser.AvatarUrl
                },
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Word: " + string.Join(" ", hangmanWord)
                }
            };
            
            var hangmanMessage = await ctx.Channel.SendMessageAsync(embed: hangmanEmbed).ConfigureAwait(false);
            
            var userGuess = await ctx.Channel.GetMessagesAsync(1).ConfigureAwait(false);
            var hangmanResult = userGuess.FirstOrDefault();
            
            if (hangmanResult.Content == word)
            {
                await ctx.Channel.SendMessageAsync("You guessed the word!").ConfigureAwait(false);
            }
            else
            {
                if (word.Contains(hangmanResult.Content))
                {
                    await ctx.Channel.SendMessageAsync("The word contains that letter!").ConfigureAwait(false);
                    
                    for (var i = 0; i < word.Length; i++)
                    {
                        if (word[i] == hangmanResult.Content[0])
                        {
                            hangmanWord[i] = hangmanResult.Content;
                        }
                    }
                    
                    var hangmanEmbed2 = new DiscordEmbedBuilder
                    {
                        Title = "Hangman",
                        Description = $"{ctx.Member.Mention}",
                        Color = DiscordColor.Turquoise,
                        Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                        {
                            Url = ctx.Client.CurrentUser.AvatarUrl
                        },
                        Footer = new DiscordEmbedBuilder.EmbedFooter
                        {
                            Text = $"Word: " + string.Join(" ", hangmanWord)
                        }
                    };
                    
                    await hangmanMessage.ModifyAsync(embed: new Optional<DiscordEmbed>(hangmanEmbed2)).ConfigureAwait(false);
                    
                }
                else
                {
                    await ctx.Channel.SendMessageAsync("The word does not contain that letter!").ConfigureAwait(false);
                }
            }
        }*/

    }

    public class Joke
    {
        public string type { get; set; }
        public string joke { get; set; }
        public string setup { get; set; }
        public string delivery { get; set; }
        public int id { get; set; }
        public Flags flags { get; set; }
    }

    public class Flags
    {
        public bool nsfw { get; set; }
        public bool religious { get; set; }
        public bool political { get; set; }
        public bool racist { get; set; }
        public bool sexist { get; set; }
    }

    public class Trivia
    {
        public string category { get; set; }
        public string type { get; set; }
        public string difficulty { get; set; }
        public string question { get; set; }
        public string correct_answer { get; set; }
        public List<string> incorrect_answers { get; set; }
    }

    public class TriviaResponse
    {
        public int response_code { get; set; }
        public List<Trivia> results { get; set; }
    }
}
