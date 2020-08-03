using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;

namespace TaricSharp.Messages
{
    public class TimerMessage : UserListMessage
    {

        public readonly DateTime StartTime;
        public readonly DateTime EndTime;
        public bool IsLocked { get; private set; } = false;

        public IMessageChannel Channel => Message.Channel;

        public TimerMessage(
            RestUserMessage message,
            int minutes) : base(message)
        {
            StartTime = DateTime.Now;
            EndTime = DateTime.Now.AddMinutes(minutes);
        }

        protected override async Task UpdateMessage()
        {
            await Message.ModifyAsync(m =>
            {
                m.Content = "";
                m.Embed = MessageEmbedBuilder().Build();
            });
        }

        public async Task LockMessage()
        {
            IsLocked = true;
            await Message.RemoveAllReactionsAsync();
            await UpdateMessage();
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
                .WithTitle($"⌛ Timer")
                .AddField("Time left:", $"{Math.Round((EndTime - DateTime.Now).TotalMinutes)} minutes.")
                .AddField("Players:", "```" + UsersToString() + "```", true)
                .WithColor(IsLocked ? Color.DarkBlue : Color.DarkGreen);

            return embed;
        }

        private EmbedBuilder FinishedMessageEmbedBuilder()
        {
            return new EmbedBuilder()
                .WithTitle($"Timer finished")
                .AddField("Players:", "```" + UsersToString() + "```", true)
                .WithColor(Color.DarkRed);
        }
    }
}