using System.Threading.Tasks;
using Discord.Commands;
using TaricSharp.Services;

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
            int minutes)
        {
            // Check input is valid
            // Give user feedback
            // Start timer service

            await _timerService.CreateTimerMessage(Context, minutes);
        }
    }
}