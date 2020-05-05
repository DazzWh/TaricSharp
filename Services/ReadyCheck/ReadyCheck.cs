using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;

namespace TaricSharp.Services
{
    public class ReadyCheck
    {
        public readonly RestUserMessage ReadyMsg;
        private readonly int _readyCount;
        private readonly HashSet<IUser> _readyUsers;

        public ReadyCheck(
            RestUserMessage readyMsg,
            int readyCount)
        {
            ReadyMsg = readyMsg;
            _readyCount = readyCount;
            _readyUsers = new HashSet<IUser>();
        }

        public async Task AddReadyUser(IUser user)
        {
            if (_readyUsers.Contains(user))
                return;
            
            _readyUsers.Add(user);
            await UpdateMessage();
        }

        public async Task RemoveReadyUser(IUser user)
        {
            if (!_readyUsers.Contains(user))
                return;

            _readyUsers.Remove(user);
            await UpdateMessage();
        }

        private async Task UpdateMessage()
        {
            var text = "Ready users: ";
            foreach (var user in _readyUsers)
            {
                text += user.Username;
            }

            await ReadyMsg.ModifyAsync(m => m.Content = text);
        }
    }
}