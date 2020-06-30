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
        private Timer _timer;

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

        private Task HandleReactionsAsync(
            Cacheable<IUserMessage, ulong> message,
            ISocketMessageChannel channel,
            SocketReaction reaction)
        {
            // Todo: Handle when users hit the emoji buttons on timer or endTimer messages
            throw new NotImplementedException();
        }

        public async Task CreateTimerMessage(SocketCommandContext context, int minutes)
        {
            var msg = await context.Channel.SendMessageAsync("Creating timer...");

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