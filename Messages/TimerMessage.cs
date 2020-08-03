using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace TaricSharp.Messages
{
    public class TimerMessage : UserListMessage
    {
        public readonly DateTime EndTime;

        public IMessageChannel Channel => Message.Channel;

        public TimerMessage(
            RestUserMessage message,
            int minutes) : base(message)
        {
            EndTime = DateTime.Now.AddMinutes(minutes);
        }

        protected override async Task UpdateMessage()
        {
            await Message.ModifyAsync(m =>
            {
                m.Content = $"{Math.Round((EndTime - DateTime.Now).TotalSeconds)} Seconds left of the timer!";
                m.Embed = MessageEmbedBuilder().Build();
            });
        }

        public async Task FinishMessage()
        {
            await Message.ModifyAsync(m =>
            {
                m.Content = "";
                m.Embed = FinishedMessageEmbedBuilder().Build();
            });
        }

        private string UsersToString()
        {
            return _users.Count > 0
                ? _users.Aggregate("",
                    (current, user) =>
                        current + Environment.NewLine +
                        user.Value)
                : "--None--";
        }

        private EmbedBuilder MessageEmbedBuilder()
        {
            var embed = new EmbedBuilder()
                .WithTitle($"Timer!")
                .AddField("Players:", "```" + UsersToString() + "```", true)
                .WithColor(Color.DarkGreen);

            return embed;
        }

        private EmbedBuilder FinishedMessageEmbedBuilder()
        {
            return new EmbedBuilder()
                .WithTitle($"Timer entry finished...")
                .AddField("Players:", "```" + UsersToString() + "```", true)
                .WithColor(Color.DarkRed);
        }
    }
}