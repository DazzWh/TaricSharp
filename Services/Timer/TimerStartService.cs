using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TaricSharp.Extensions;
using TaricSharp.Messages;

namespace TaricSharp.Services.Timer
{
    public class TimerStartService
    {

        private const int CheckTimeInSeconds = 20; // How often we update messages
        private const int LockTimeInMinutes = 1;   // How long a user has to join a timer

        private readonly DiscordSocketClient _client;
        private readonly TimerEndService _timerEndService;
        private readonly HashSet<TimerStartMessage> _timerMessages;

        private System.Threading.Timer _timer;
        private readonly Emoji _acceptEmoji = new Emoji("✔️");
        private readonly Emoji _cancelEmoji = new Emoji("❌");

        public TimerStartService(
            DiscordSocketClient client,
            TimerEndService timerEndService)
        {
            _client = client;
            _timerEndService = timerEndService;
            _timerMessages = new HashSet<TimerStartMessage>();
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
        /// Creates a timer message.
        /// </summary>
        /// <param name="context">The discord context used for message creation.</param>
        /// <param name="minutes">An integer representing how long the Timer should last in minutes.</param>
        /// <returns></returns>
        public async Task CreateTimerMessage(SocketCommandContext context, int minutes)
        {
            var msg = await context.Channel.SendMessageAsync("Creating timer...");

            await msg.AddReactionAsync(_acceptEmoji);
            await msg.AddReactionAsync(_cancelEmoji);

            var timerMessage = new TimerStartMessage(msg, minutes);
            _timerMessages.Add(timerMessage);
            await timerMessage.AddUser(context.User);
        }

        /// <summary>
        /// Called whenever a reaction is created in the server.
        /// If the reaction is a an accept or cancel emoji on a TimerStartMessage adds that user as committed or not to the timer.
        /// </summary>
        private async Task HandleReactionsAsync(
            Cacheable<IUserMessage, ulong> message,
            ISocketMessageChannel channel,
            SocketReaction reaction)
        {
            var timerMessage = _timerMessages.FirstOrDefault(m => m.Id == message.Id);
            if (reaction.UserNullOrBot() || timerMessage == null)
                return;

            if (!timerMessage.IsLocked)
            {
                if (reaction.Emote.Equals(_acceptEmoji))
                    await timerMessage.AddUser(reaction.User.Value);
                
                // Todo: remove cancel emoji, and the "check halfway done" thing. Just if they're in they're in.
                if (reaction.Emote.Equals(_cancelEmoji))
                    await timerMessage.RemoveUser(reaction.User.Value);
            }

            await timerMessage.Message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
        }

        /// <summary>
        /// Called by a timer every few seconds to make sure the timers are up to date
        /// </summary>
        private async Task CheckMessages()
        {
            // TODO: Make sure Message exists
            foreach (var msg in _timerMessages)
            {
                await msg.UpdateMessage();
            }

            await LockMessages();
            await FinishMessages();
            
        }

        /// <summary>
        /// Messages are locked after a certain time
        /// This way people can't just back out after saying they will be there at that time.
        /// </summary>
        private async Task LockMessages()
        {
            foreach (var msg in
                _timerMessages.Where(msg => msg.StartTime.AddMinutes(LockTimeInMinutes) < DateTime.Now))
                await msg.LockMessage();
        }

        /// <summary>
        /// Checks for messages that are past their EndTime date
        /// Calls FinishMessage and creates an EndTimerMessage for each completed timer.
        /// </summary>
        private async Task FinishMessages()
        {
            var finishedTimers =
                _timerMessages.Where(msg => msg.EndTime < DateTime.Now)
                    .ToArray();

            foreach (var msg in finishedTimers)
            {
                // Create an end message
                await _timerEndService.CreateEndTimerMessage(msg);
                await msg.FinishMessage();
                _timerMessages.Remove(msg);
            }
        }
    }
}