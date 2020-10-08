using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using TaricSharp.Messages;

namespace TaricSharp.Services.Timer
{
    public class TimerStartService : TimerService
    {
        private readonly TimerEndService _timerEndService;

        public TimerStartService(
            DiscordSocketClient client,
            TimerEndService timerEndService) 
            : base(client)
        {
            _timerEndService = timerEndService;
        }
        
        public async Task CreateTimerMessage(SocketCommandContext context, int minutes)
        {
            var msg = await context.Channel.SendMessageAsync("Creating timer...");

            await msg.AddReactionAsync(AcceptEmoji);

            var timerMessage = new TimerStartMessage(msg, minutes);
            Messages.Add(timerMessage);
            await timerMessage.AddUser(context.User);
        }

        protected override async Task OnMessageComplete(TimerMessage msg)
        {
            if (msg is TimerStartMessage startMessage)
            {
                await _timerEndService.CreateEndTimerMessage(startMessage);
            }
        }
    }
}