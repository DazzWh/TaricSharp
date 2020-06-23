using System.Threading.Tasks;
using Discord.Commands;

namespace TaricSharp.Modules
{
    public class TimerModule : ModuleBase<SocketCommandContext>
    {
        private readonly TimerService _timerService;

        public TimerModule(
            TimerService timerService)
        {
            _timerService = timerService;
        }

        [Command("timer")]
        [Summary("Initiates a timer")]
        [Remarks("")]
        public async Task InitiateTimer(
            int mins)
        {
            // Check input is valid
            // Give user feedback
            // Start timer service

            _timerService.CreateTimerMessage(Context, mins);
        }
    }
}