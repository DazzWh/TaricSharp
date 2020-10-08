using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TaricSharp.Services.Timer;
using TaricSharp.Services.Timer.Data;

namespace TaricSharp.Modules
{
    public class TimerModule : ModuleBase<SocketCommandContext>
    {
        private readonly TimerStartService _timerStartService;
        private readonly LateUserDataService _lateUserDataService;

        public TimerModule(
            TimerStartService timerStartService,
            LateUserDataService lateUserDataService)
        {
            _timerStartService = timerStartService;
            _lateUserDataService = lateUserDataService;
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
            var users = _lateUserDataService.GetUsersFromGuild(Context.Guild.Id);
            
            
            var scores = 
                (from userData in users
                    .Where(u => Context.Guild.GetUser(u.Id) != null) 
                    let user = Context.Guild.GetUser(userData.Id) 
                    select new Tuple<string, LateUser>(user.Username, userData)).ToList();
            
            var embed = new EmbedBuilder()
                .AddField("⌛ Late users", $"```{ScoresFormatted(scores)}```", true)
                .WithColor(Color.DarkBlue);

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        private static string ScoresFormatted(IReadOnlyCollection<Tuple<string, LateUser>> scores)
        {
            if (scores.Count == 0) 
                return "-- No Data For Server --";

            var sb = new StringBuilder();
            sb.Append("Name : OnTimeCount : LateCount");
            scores.ToList()
                .ForEach(score=> sb.AppendLine($"{score.Item1} : {score.Item2.OnTimeCount} : {score.Item2.LateCount}"));
            return sb.ToString();
        }
    }
}