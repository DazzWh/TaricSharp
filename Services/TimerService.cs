using System.Collections.Generic;
using System.Threading;
using Discord.Commands;

namespace TaricSharp.Services
{
    public class TimerService
    {

        // List of end times and messages
        private List<TimerMessage> _timerMessages;

        // private timer
        private Timer _timer;

        public TimerService()
        {
            _timerMessages = new List<TimerMessage>();
        }

        public void CreateTimerMessage(SocketCommandContext context, in int mins)
        {
            // Subscribe to the event if there are no times currently being done



            throw new System.NotImplementedException();
        }


        private void EndTimerMessage()
        {
            // Send message

            // Remove from list

            // Unsubscribe if list empty
        }

        /// <summary>
        /// Creates a timer event that checks the list of timers periodically
        /// </summary>
        private void StartTimerEvent()
        {

        }

        /// <summary>
        /// Ends the periodic checking of the timer list if no timers are active
        /// </summary>
        private void EndTimerEvent()
        {

        }
    }
}