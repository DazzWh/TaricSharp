using System.Collections.Generic;
using Discord;

namespace TaricSharp.Services
{
    public class ReadyCheck
    {
        private string _embedMessageID;
        private int _readyCount;
        private HashSet<IUser> _readyUsers;

        public ReadyCheck(
            string embedMessageId,
            int readyCount,
            IUser commandAuthor)
        {
            _embedMessageID = embedMessageId;
            _readyCount = readyCount;
            _readyUsers = new HashSet<IUser>{commandAuthor};
            UpdateMessage();
        }

        public void AddReadyUser(IUser readyUser){}

        public void RemoveReadyUser(IUser readyUser){ }

        private void UpdateMessage(){}

    }
}