using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;

namespace TaricSharp.Messages
{
    public class TimerEndMessage : UserListMessage
    {
        public readonly DateTime EndTime;
        public readonly ulong GuildId;
        public bool Finished = false;

        private readonly Dictionary<ulong, string> _onTimeUsers;

        public TimerEndMessage(
            RestUserMessage message,
            int secondsTillEnd,
            Dictionary<ulong, string> initialUsers,
            ulong guildId
            ) : base(message)
        {
            GuildId = guildId;
            EndTime = DateTime.Now.AddSeconds(secondsTillEnd);
            _onTimeUsers = new Dictionary<ulong, string>();

            foreach (var (key, value) in initialUsers)
            {
                Users.TryAdd(key, value);
            }
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
            await Message.RemoveAllReactionsAsync();

            Finished = true;
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
            // Todo: Make this show the late count on the message, inject LateUserDataService
            return new EmbedBuilder()
                .WithTitle("Late Report")
                .AddField("Late: ", "```" + UsersToString() + "```", true)
                .WithColor(Color.DarkRed);
        }
    }
}