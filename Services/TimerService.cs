using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TaricSharp.Messages;

namespace TaricSharp.Services
{
    public class TimerService
    {
        private readonly DiscordSocketClient _client;
        private readonly HashSet<TimerMessage> _timerMessages;
        private readonly HashSet<TimerEndMessage> _endMessages;
        private readonly Timer _timer;

        private readonly Emoji _acceptEmoji = new Emoji("✔️");
        private readonly Emoji _cancelEmoji = new Emoji("❌");

        public TimerService(
            DiscordSocketClient client)
        {
            _client = client;
            _timerMessages = new HashSet<TimerMessage>();
            _endMessages = new HashSet<TimerEndMessage>();

            _timer = new Timer(
                e => CheckMessages(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromMinutes(1));
        }

        public void Initialize()
        {
            _client.ReactionAdded += HandleReactionsAsync;
        }

        private async Task HandleReactionsAsync(
            Cacheable<IUserMessage, ulong> message,
            ISocketMessageChannel channel,
            SocketReaction reaction)
        {
            if (!reaction.User.IsSpecified || reaction.User.Value.IsBot)
                return;

            var msg = await channel.GetMessageAsync(message.Id);
            
            var timerMessage = _timerMessages.FirstOrDefault(m => m.Id == msg.Id);
            var endMessage = _endMessages.FirstOrDefault(m => m.Id == msg.Id);

            if (timerMessage == null && endMessage == null)
                return;

            if (timerMessage != null)
            {
                // Handle timer messages reactions

            }

            if (endMessage != null)
            {
                // Handle end messages reactions
            }
        }

        public async Task CreateTimerMessage(SocketCommandContext context, int minutes)
        {
            var msg = await context.Channel.SendMessageAsync("Creating timer...");

            await msg.AddReactionAsync(_acceptEmoji);
            await msg.AddReactionAsync(_cancelEmoji);

            _timerMessages.Add(new TimerMessage(msg, minutes));
            throw new NotImplementedException();
        }

        private void CheckMessages()
        {
            foreach (var msg in
                _timerMessages.Where(msg => msg.EndTime < DateTime.Now))
            {
                _endMessages.Add(new TimerEndMessage());
                msg.FinishMessage();
                _timerMessages.Remove(msg);
            }

            foreach (var msg in
                _endMessages.Where(msg => msg.EndTime < DateTime.Now))
            {
                msg.FinishMessage();
                // Increment the database of users who didn't accept in time
                _endMessages.Remove(msg);
            }
        }
    }
}