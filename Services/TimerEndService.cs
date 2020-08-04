using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using TaricSharp.Messages;

namespace TaricSharp.Services
{
    public class TimerEndService
    {
        private const int CheckTimeInSeconds = 30; // How often we update messages
        private const int HereTimeInSeconds = 60; // How long a user has to say they're here

        private readonly HashSet<TimerEndMessage> _endMessages;

        private Timer _timer;
        private readonly Emoji _acceptEmoji = new Emoji("✔️");
        private readonly DiscordSocketClient _client;

        public TimerEndService(
            DiscordSocketClient client)
        {
            _client = client;
            _endMessages = new HashSet<TimerEndMessage>();
        }

        public void Initialize()
        {
            _client.ReactionAdded += HandleReactionsAsync;

            _timer = new Timer(async e => await CheckMessages(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(CheckTimeInSeconds));
        }

        /// <summary>
        /// Creates an EndTimerMessage from a finished TimerMessage.
        /// </summary>
        /// <param name="tMsg">The Timer Message that has ended to create the EndTimerMessage from.</param>
        public async Task CreateEndTimerMessage(TimerMessage tMsg)
        {
            var msg = (RestUserMessage) await tMsg.Message.Channel.SendMessageAsync("Creating timer complete message...");

            await msg.AddReactionAsync(_acceptEmoji);

            _endMessages.Add(new TimerEndMessage(msg));
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
            if (!reaction.User.IsSpecified || reaction.User.Value.IsBot)
                return;

            var msg = await channel.GetMessageAsync(message.Id);

            var endMessage = _endMessages.FirstOrDefault(m => m.Id == msg.Id);

            if (endMessage == null)
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
                    .Where(msg => msg.Message.Timestamp.AddSeconds(HereTimeInSeconds) < DateTime.Now)
                    .ToArray();

            foreach (var msg in finishedEndMessages)
            {
                await msg.FinishMessage();
                //TODO: Log who was late on a scoreboard.
                _endMessages.Remove(msg);
            }
        }
    }
}