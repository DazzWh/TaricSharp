using System;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;

namespace TaricSharp.Messages
{
    public abstract class TimerMessage : UserListMessage
    {
        public readonly DateTime EndTime;

        protected TimerMessage(
            RestUserMessage message,
            int minutes)
            : base(message)
        {
            EndTime = DateTime.Now.AddSeconds(minutes); //Todo: make this mins for real thing
        }
        
        public override async Task UpdateMessage()
        {
            var embed = EndTime > DateTime.Now ? CountdownMessageEmbedBuilder() : FinishedMessageEmbedBuilder();
            
            await Message.ModifyAsync(m =>
            {
                m.Content = "";
                m.Embed = embed.Build();
            });
        }

        protected abstract EmbedBuilder CountdownMessageEmbedBuilder();
        protected abstract EmbedBuilder FinishedMessageEmbedBuilder();
    }
}