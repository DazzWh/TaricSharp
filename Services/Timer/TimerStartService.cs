using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using TaricSharp.Messages;
using TaricSharp.Services.Games;

namespace TaricSharp.Services.Timer
{
    public class TimerStartService : TimerService
    {
        private readonly TimerEndService _timerEndService;
        private readonly GameService _gameService;

        public TimerStartService(
            DiscordSocketClient client,
            TimerEndService timerEndService,
            GameService gameService) 
            : base(client)
        {
            _timerEndService = timerEndService;
            _gameService = gameService;
        }
        
        public async Task CreateTimerMessage(SocketCommandContext context, int minutes)
        {
            var msg = await context.Channel.SendMessageAsync("Creating timer...");
            
            await msg.AddReactionAsync(AcceptEmoji);
            
            var gameInfo = _gameService.GetGameFromMentions(context.Message.MentionedRoles);
            var timerMessage = new TimerStartMessage(msg, minutes, gameInfo);
            await timerMessage.AddUser(context.User);
            Messages.Add(timerMessage);
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