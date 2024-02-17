using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;

namespace Ker_NelBot.Handlers.Dialogue.Steps
{
    public class ReactionStep : DialogueStepBase
    {
        private readonly Dictionary<DiscordEmoji, ReactionStepData> _options;
        
        private DiscordEmoji _selectedEmoji;

        public ReactionStep(string content, Dictionary<DiscordEmoji, ReactionStepData> options) : base (content)
        {
            _options = options;
        }

        public override IDialogueStep NextStep
        {
            get => _options[_selectedEmoji].NextStep;
            private protected set => _options[_selectedEmoji].NextStep = value;
        }

        public Action<DiscordEmoji> OnValidResult { get; set; } = delegate {  };

        public async override Task<bool> ProcessStep(DiscordClient client, DiscordChannel channel, DiscordUser user)
        {
            var cancelEmoji = DiscordEmoji.FromName(client, ":x:");
            
            var embedBuilder = new DiscordEmbedBuilder
            {
                Title = $"Please react below.",
                Description = $"{user.Mention}, {_content}",
                Color = DiscordColor.Blurple
            };
            
            embedBuilder.AddField("To stop the dialogue", "React with the :x: emoji.");
            
            var interactivity = client.GetInteractivity();
            
            while (true)
            {
                var embed = await channel.SendMessageAsync(embed: embedBuilder).ConfigureAwait(false);

                OnMessageAdded(embed);
                
                foreach (var emoji in _options.Keys)
                {
                    await embed.CreateReactionAsync(emoji).ConfigureAwait(false);
                }
                
                await embed.CreateReactionAsync(cancelEmoji).ConfigureAwait(false);

                var reactionResult = await interactivity.WaitForReactionAsync(
                    x => _options.ContainsKey(x.Emoji) || x.Emoji == cancelEmoji,
                    embed,
                    user).ConfigureAwait(false);
                
                if (reactionResult.Result.Emoji == cancelEmoji)
                {
                    return true;
                }

                _selectedEmoji = reactionResult.Result.Emoji;

                OnValidResult(_selectedEmoji);

                return false;
            }
        }
    }

    public class ReactionStepData
    {
        public IDialogueStep NextStep { get; set; }
        public string Content { get; set; }
    }
}