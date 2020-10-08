using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using MoreLinq;

namespace TaricSharp.Messages
{
    public class TimerEndMessage : TimerMessage
    {
        public readonly ulong GuildId;

        private readonly Dictionary<ulong, string> _onTimeUsers;

        public TimerEndMessage(
            RestUserMessage message,
            int minutes,
            Dictionary<ulong, string> initialUsers,
            ulong guildId) 
            : base(message, minutes)
        {
            GuildId = guildId;
            _onTimeUsers = new Dictionary<ulong, string>();
            initialUsers.ForEach(u => Users.TryAdd(u.Key, u.Value));
        }

        public override async Task AddUser(
            IUser user)
        {
            if (!Users.ContainsKey(user.Id))
                return;

            _onTimeUsers.TryAdd(user.Id, user.Username);
            Users.Remove(user.Id);
            await UpdateMessage();
        }

        protected override EmbedBuilder CountdownMessageEmbedBuilder()
        {
            var embed = new EmbedBuilder()
                .WithTitle($"⌛ Checking who is actually here...")
                .AddField("Time Remaining:", $"{Math.Round((EndTime - DateTime.Now).TotalSeconds)} seconds.")
                .AddField("Here:", $"```{UsersToString(_onTimeUsers)}```", true)
                .AddField("Should be here:", $"```{UsersToString()}```")
                .WithColor(Color.DarkOrange);

            return embed;
        }

        protected override EmbedBuilder FinishedMessageEmbedBuilder()
        {
            // Todo: Make this show the late count on the message, inject LateUserDataService
            return new EmbedBuilder()
                .WithTitle("Late Report")
                .AddField("Late: ", "```" + UsersToString() + "```", true)
                .WithColor(Color.DarkRed);
        }
    }
}