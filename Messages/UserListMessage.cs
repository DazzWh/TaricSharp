using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;

namespace TaricSharp.Messages
{
    public abstract class UserListMessage
    {
        protected readonly RestUserMessage _message;
        protected readonly Dictionary<ulong, string> _users = new Dictionary<ulong, string>();

        protected UserListMessage(RestUserMessage message)
        {
            _message = message;
        }

        public ulong Id => _message.Id;

        public virtual async Task AddUser(
            IUser user)
        {
            _users.TryAdd(user.Id, user.Username);
            await UpdateMessage();
        }

        public virtual async Task RemoveUser(
            IUser user)
        {
            _users.Remove(user.Id);
            await UpdateMessage();
        }

        protected abstract Task UpdateMessage();
        public override int GetHashCode() => _message.GetHashCode();
    }
}