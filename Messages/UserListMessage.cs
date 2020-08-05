using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;

namespace TaricSharp.Messages
{
    public abstract class UserListMessage
    {
        public readonly RestUserMessage Message;
        public readonly Dictionary<ulong, string> Users = new Dictionary<ulong, string>();

        protected UserListMessage(RestUserMessage message)
        {
            Message = message;
        }

        public ulong Id => Message.Id;

        public virtual async Task AddUser(
            IUser user)
        {
            Users.TryAdd(user.Id, user.Username);
            await UpdateMessage();
        }

        public virtual async Task AddUser(
            ulong id,
            string username)
        {
            Users.TryAdd(id, username);
            await UpdateMessage();
        }

        public virtual async Task RemoveUser(
            IUser user)
        {
            Users.Remove(user.Id);
            await UpdateMessage();
        }
        
        protected string UsersToString(Dictionary<ulong, string> userList)
        {
            return userList.Count > 0
                ? userList.Aggregate("",
                    (current, user) =>
                        current + Environment.NewLine +
                        user.Value)
                : "--None--";
        }
        protected string UsersToString()
        {
            return UsersToString(Users);
        }

        public abstract Task UpdateMessage();
        public override int GetHashCode() => Message.GetHashCode();
    }
}