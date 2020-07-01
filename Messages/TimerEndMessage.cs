using System;

namespace TaricSharp.Messages
{
    public class TimerEndMessage : UserListMessage
    {
        public DateTime EndTime { get; set; }
        public ulong Id { get; set; }

        public void FinishMessage()
        {
            throw new NotImplementedException();
        }
    }
}