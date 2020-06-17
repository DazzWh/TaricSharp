using System;
using System.Collections.Generic;
using System.Threading;
using Discord.Commands;

namespace TaricSharp.Services
{
    public class TimerService
    {
        private HashSet<TimerMessage> _timerMessages;
        private HashSet<TimerEndMessage> _endMessages;
        private Timer _timer;

        public TimerService()
        {
            _timerMessages = new HashSet<TimerMessage>();
            _endMessages = new HashSet<TimerEndMessage>();
            _timer = new Timer(
                e => CheckMessages(),  
                null, 
                TimeSpan.Zero, 
                TimeSpan.FromMinutes(1));
        }

        private void CheckMessages()
        {
            foreach (var msg in _timerMessages)
            {
                // If message due date is lower than time now
                    // Create end message
                    // Remove from _timerMessages
            }

            foreach (var msg in _endMessages)
            {
                // If message final time is lower than now time
                    // Update end message
                    // Add to note users who didn't accept in time
                    // Remove from _endMessages
            }
        }

        public void CreateTimerMessage(SocketCommandContext context, in int mins)
        {
            // TODO: Subscribe to the event if there are no times currently being done

            throw new NotImplementedException();
        }


        private void EndTimerMessage()
        {
            // Send message

            // Remove from list

            // Unsubscribe if list empty
        }
    }
}