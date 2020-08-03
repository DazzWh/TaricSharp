using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;

namespace TaricSharp.Messages
{
    public abstract class UserListMessage
    {
        public readonly RestUserMessage Message;
        protected readonly Dictionary<ulong, string> _users = new Dictionary<ulong, string>();

        protected UserListMessage(RestUserMessage message)
        {
            Message = message;
        }

        public ulong Id => Message.Id;

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
        public override int GetHashCode() => Message.GetHashCode();
    }
}