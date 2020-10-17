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
        public readonly Dictionary<ulong, string> OnTimeUsers;

        public TimerEndMessage(
            RestUserMessage message,
            int minutes,
            Dictionary<ulong, string> initialUsers,
            ulong guildId) 
            : base(message, minutes)
        {
            GuildId = guildId;
            OnTimeUsers = new Dictionary<ulong, string>();
            initialUsers.ForEach(u => Users.TryAdd(u.Key, u.Value));
        }

        public override async Task AddUser(
            IUser user)
        {
            if (!Users.ContainsKey(user.Id))
                return;

            OnTimeUsers.TryAdd(user.Id, user.Username);
            Users.Remove(user.Id);
            await UpdateMessage();
        }

        protected override EmbedBuilder CountdownMessageEmbedBuilder()
        {
            var embed = new EmbedBuilder()
                .WithTitle($"⌛ Who's here?")
                .AddField("Time Remaining:", $"{Math.Round((EndTime - DateTime.Now).TotalSeconds)} seconds.")
                .AddField("Here:", $"```{UsersToString(OnTimeUsers)}```", true)
                .AddField("Should be here:", $"```{UsersToString()}```")
                .WithColor(Color.DarkOrange);

            return embed;
        }

        protected override EmbedBuilder FinishedMessageEmbedBuilder()
        {
            return new EmbedBuilder()
                .AddField("Late report: ", "```" + UsersToString() + "```", true)
                .WithColor(Color.DarkRed);
        }
    }
}