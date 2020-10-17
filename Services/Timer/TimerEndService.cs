using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using TaricSharp.Messages;
using TaricSharp.Services.Timer.Data;

namespace TaricSharp.Services.Timer
{
    public class TimerEndService : TimerService
    {
        private const int HereTimeInMinutes = 1; 

        private readonly LateUserDataService _dataService;
        private readonly LoggingService _loggingService;

        public TimerEndService(
            DiscordSocketClient client,
            LateUserDataService dataService,
            LoggingService loggingService)
            : base(client)
        {
            _dataService = dataService;
            _loggingService = loggingService;
        }

        public async Task CreateEndTimerMessage(TimerStartMessage timerStart)
        {
            var msg = (RestUserMessage) await timerStart.Message.Channel.SendMessageAsync("Creating timer complete message...");
            await msg.AddReactionAsync(AcceptEmoji);

            if (!(msg.Channel is SocketGuildChannel channel))
            {
                await _loggingService.Log(new LogMessage(
                    LogSeverity.Error, 
                    nameof(TimerEndService), 
                    "Failed to cast TimerStartMessage channel to SocketGuildChannel"));
                await msg.ModifyAsync(m => m.Content = "Failed to create TimerEndMessage");
                return;
            }
            
            var timerEndMessage = new TimerEndMessage(msg, HereTimeInMinutes, timerStart.Users, channel.Guild.Id);
            await timerEndMessage.UpdateMessage();
            Messages.Add(timerEndMessage);
        }

        protected override async Task OnMessageComplete(TimerMessage msg)
        {
            if(msg is TimerEndMessage endMessage)
            {
                await _dataService.IncrementLateUsers(endMessage.Users.Keys, endMessage.GuildId);
                await _dataService.IncrementOnTimeUsers(endMessage.OnTimeUsers.Keys, endMessage.GuildId);
            }
        }
    }
}