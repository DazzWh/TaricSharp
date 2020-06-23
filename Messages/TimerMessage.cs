using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;

namespace TaricSharp.Messages
{
    public class TimerMessage
    {
        private readonly RestUserMessage _message;
        private readonly Dictionary<ulong, string> _users;

        public TimerMessage(
            RestUserMessage message)
        {
            _message = message;
        }

        public void AddUser(
            IUser user)
        {
            _users.TryAdd(user.Id, user.Username);
            await UpdateMessage();
        }

        public void RemoveUser()
        {

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
    }
}