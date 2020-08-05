using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;

namespace TaricSharp.Messages
{
    public class TimerEndMessage : UserListMessage
    {

        public readonly DateTime EndTime;

        private readonly Dictionary<ulong, string> _onTimeUsers;

        public TimerEndMessage(
            RestUserMessage message,
            int secondsTillEnd
            ) : base(message)
        {
            EndTime = DateTime.Now.AddSeconds(secondsTillEnd);
            _onTimeUsers = new Dictionary<ulong, string>();
        }

        public override async Task UpdateMessage()
        {
            await Message.ModifyAsync(m =>
            {
                m.Content = "";
                m.Embed = MessageEmbedBuilder().Build();
            });
        }

        public override async Task AddUser(
            IUser user)
        {
            _onTimeUsers.TryAdd(user.Id, user.Username);
            Users.Remove(user.Id);
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

        private EmbedBuilder MessageEmbedBuilder()
        {
            var embed = new EmbedBuilder()
                .WithTitle($"⌛ Checking who is actually here...")
                .AddField("Time Remaining:", $"{Math.Round((EndTime - DateTime.Now).TotalSeconds)} seconds.")
                .AddField("Here:", $"```{UsersToString(_onTimeUsers)}```", true)
                .AddField("Should be here:", $"```{UsersToString()}```")
                .WithColor(Color.DarkOrange);

            return embed;
        }

        private EmbedBuilder FinishedMessageEmbedBuilder()
        {
            return new EmbedBuilder()
                .WithTitle("Late Report")
                .AddField("Late: ", "```" + UsersToString() + "```", true)
                .WithColor(Color.DarkRed);
        }
    }
}