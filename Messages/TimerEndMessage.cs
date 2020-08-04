using System;
using System.Threading.Tasks;
using Discord.Rest;

namespace TaricSharp.Messages
{
    public class TimerEndMessage : UserListMessage
    {
        public DateTime EndTime { get; set; }

        public TimerEndMessage(RestUserMessage message) : base(message)
        {
            EndTime = DateTime.Now.AddMinutes(1);
        }

        protected override Task UpdateMessage()
        {
            throw new NotImplementedException();
        }

        public async Task FinishMessage()
        {
            throw new NotImplementedException();
        }

    }
}