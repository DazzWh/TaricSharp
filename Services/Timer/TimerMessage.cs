using System.Collections.Generic;
using Discord.Rest;

namespace TaricSharp.Services
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

        public void UpdateMessage()
        {

        }

        public void AddUser()
        {

        }

        public void RemoveUser()
        {

        }
    }
}