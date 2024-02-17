using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;

namespace Ker_NelBot.Handlers.Dialogue.Steps
{
    public abstract class DialogueStepBase : IDialogueStep
    {
        protected readonly string _content;
        
        public DialogueStepBase(string content)
        {
            _content = content;
        }
        
        public Action<DiscordMessage> OnMessageAdded { get; set; } = delegate {  };
        public abstract IDialogueStep NextStep { get; private protected set; }
        
        public abstract Task<bool> ProcessStep(DiscordClient client, DiscordChannel channel, DiscordUser user);
        
        protected async Task TryAgain(DiscordChannel channel, string problem)
        {
            var embedBuilder = new DiscordEmbedBuilder
            {
                Title = "Please try again.",
                Description = $"{problem}\n\n{_content}",
                Color = DiscordColor.Red
            };
            
            var embed = await channel.SendMessageAsync(embed: embedBuilder).ConfigureAwait(false);
            
            OnMessageAdded(embed);
        }
    }
}