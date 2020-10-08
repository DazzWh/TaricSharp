using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using TaricSharp.Extensions;
using TaricSharp.Messages;
using TaricSharp.Services.Timer.Data;

namespace TaricSharp.Services.Timer
{
    public class TimerEndService
    {
        //Todo: Revert these timers
        private const int CheckTimeInSeconds = 5; // How often we update messages
        private const int HereTimeInSeconds = 5; // How long a user has to say they're here

        private readonly HashSet<TimerEndMessage> _endMessages;

        private System.Threading.Timer _timer;
        private readonly Emoji _acceptEmoji = new Emoji("✔️");
        private readonly DiscordSocketClient _client;
        private readonly LateUserDataService _dataService;
        private readonly LoggingService _loggingService;

        public TimerEndService(
            DiscordSocketClient client,
            LateUserDataService dataService,
            LoggingService loggingService)
        {
            _client = client;
            _dataService = dataService;
            _loggingService = loggingService;
            _endMessages = new HashSet<TimerEndMessage>();
        }

        public void Initialize()
        {
            _client.ReactionAdded += HandleReactionsAsync;

            _timer = new System.Threading.Timer(async e => await CheckMessages(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(CheckTimeInSeconds));
        }

        /// <summary>
        /// Creates an EndTimerMessage from a finished TimerStartMessage.
        /// </summary>
        /// <param name="timerStart">The Timer Message that has ended to create the EndTimerMessage from.</param>
        public async Task CreateEndTimerMessage(TimerStartMessage timerStart)
        {
            var msg = (RestUserMessage) await timerStart.Message.Channel.SendMessageAsync("Creating timer complete message...");
            await msg.AddReactionAsync(_acceptEmoji);

            if (!(msg.Channel is SocketGuildChannel channel))
            {
                await _loggingService.Log(new LogMessage(
                    LogSeverity.Error, 
                    nameof(TimerEndService), 
                    $"Failed to cast TimerStartMessage channel to SocketGuildChannel"));
                await msg.ModifyAsync(m => m.Content = "Failed to create TimerEndMessage");
                return;
            }
            
            var timerEndMessage = new TimerEndMessage(msg, HereTimeInSeconds, timerStart.Users, channel.Guild.Id);
            _endMessages.Add(timerEndMessage);
            await timerEndMessage.UpdateMessage();
        }

        /// <summary>
        /// Called whenever a reaction is created in the server.
        /// If the reaction is a an accept emoji on an EndTimerMessage adds that user as here on time.
        /// </summary>
        private async Task HandleReactionsAsync(
            Cacheable<IUserMessage, ulong> message,
            ISocketMessageChannel channel,
            SocketReaction reaction)
        {
            var endMessage = _endMessages.FirstOrDefault(m => m.Id == message.Id);
            if (reaction.UserNullOrBot() || endMessage == null)
                return;
            
            if (reaction.Emote.Equals(_acceptEmoji))
                await endMessage.AddUser(reaction.User.Value);

            await endMessage.Message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
        }
        
        /// <summary>
        /// Checks for messages that are past their cutoff point
        /// Calls FinishMessage and logs all people who are late on any finished messages.
        /// </summary>
        private async Task CheckMessages()
        {
            var finishedEndMessages =
                _endMessages
                    .Where(msg => msg.EndTime.AddSeconds(HereTimeInSeconds) < DateTime.Now)
                    .ToArray();

            foreach (var msg in finishedEndMessages)
            {
                await msg.FinishMessage();
                await _dataService.IncrementLateUsers(msg.Users.Keys, msg.GuildId);
            }

            _endMessages.RemoveWhere(msg => msg.Finished);
        }
    }
}