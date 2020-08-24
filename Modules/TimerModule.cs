using System.Threading.Tasks;
using Discord.Commands;
using TaricSharp.Services;

namespace TaricSharp.Modules
{
    public class TimerModule : ModuleBase<SocketCommandContext>
    {
        private readonly TimerStartService _timerStartService;

        public TimerModule(
            TimerStartService timerStartService)
        {
            _timerStartService = timerStartService;
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

            await _timerStartService.CreateTimerMessage(Context, minutes);
        }

        [Command("late")]
        [Summary("Shows how many times users have been late")]
        public async Task LateScoreboard()
        {
            //TODO: Add this command
            await ReplyAsync("Insert late leaderboard here...");
        }
    }
}