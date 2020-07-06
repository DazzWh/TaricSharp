using System;
using System.Threading.Tasks;
using Discord.Rest;

namespace TaricSharp.Messages
{
    public class TimerEndMessage : UserListMessage
    {
        public DateTime EndTime { get; set; }

        protected override Task UpdateMessage()
        {
            throw new NotImplementedException();
        }

        public void FinishMessage()
        {
            throw new NotImplementedException();
        }

        public TimerEndMessage(RestUserMessage message) : base(message)
        {
        }
    }
}