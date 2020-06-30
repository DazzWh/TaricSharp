using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;

namespace TaricSharp.Messages
{
    public class TimerMessage
    {
        private readonly RestUserMessage _message;
        private readonly Dictionary<ulong, string> _users = new Dictionary<ulong, string>();

        public readonly DateTime EndTime;

        public TimerMessage(
            RestUserMessage message,
            int minutes)
        {
            _message = message;
            EndTime = DateTime.Now.AddMinutes(minutes);
        }

        public async Task AddUser(
            IUser user)
        {
            _users.TryAdd(user.Id, user.Username);
            await UpdateMessage();
        }

        public async Task RemoveUser(
            IUser user)
        {
            _users.Remove(user.Id);
            await UpdateMessage();
        }

        private async Task UpdateMessage()
        {
            var embed = MessageEmbedBuilder();

            await _message.ModifyAsync(m =>
            {
                m.Content = "";
                m.Embed = embed.Build();
            });
        }

        public void FinishMessage()
        {
            throw new NotImplementedException();
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
                .AddField("Players:", "```" + UsersToString() + "```", true);

            return embed;
        }

        public override int GetHashCode() => _message.GetHashCode();

        
    }
}