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
    public class TimerService
    {
        private readonly DiscordSocketClient _client;
        private readonly HashSet<TimerMessage> _timerMessages;
        private readonly HashSet<TimerEndMessage> _endMessages;

        private Timer _timer;
        private readonly Emoji _acceptEmoji = new Emoji("✔️");
        private readonly Emoji _cancelEmoji = new Emoji("❌");

        public TimerService(
            DiscordSocketClient client)
        {
            _client = client;
            _timerMessages = new HashSet<TimerMessage>();
            _endMessages = new HashSet<TimerEndMessage>();
        }

        public void Initialize()
        {
            _client.ReactionAdded += HandleReactionsAsync;

            _timer = new Timer(async e => await CheckMessages(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(30));
        }

        public async Task CreateTimerMessage(SocketCommandContext context, int minutes)
        {
            var msg = await context.Channel.SendMessageAsync("Creating timer...");

            await msg.AddReactionAsync(_acceptEmoji);
            await msg.AddReactionAsync(_cancelEmoji);

            var timerMessage = new TimerMessage(msg, minutes);
            _timerMessages.Add(timerMessage);
            await timerMessage.AddUser(context.User);
        }

        private async Task HandleReactionsAsync(
            Cacheable<IUserMessage, ulong> message,
            ISocketMessageChannel channel,
            SocketReaction reaction)
        {
            // Todo: Abstract this class into a "message service" class so this code isn't written 3 times.
            if (!reaction.User.IsSpecified || reaction.User.Value.IsBot)
                return;

            var msg = await channel.GetMessageAsync(message.Id);
            var timerMessage = _timerMessages.FirstOrDefault(m => m.Id == msg.Id);

            if (timerMessage == null)
                return;

            if (reaction.Emote.Equals(_acceptEmoji))
                await timerMessage.AddUser(reaction.User.Value);

            if (reaction.Emote.Equals(_cancelEmoji))
                await timerMessage.RemoveUser(reaction.User.Value);

            await timerMessage.Message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
        }

        private async Task CheckMessages()
        {
            var finishedTimers = 
                _timerMessages.Where(msg => msg.EndTime < DateTime.Now)
                .ToArray();

            foreach (var msg in finishedTimers)
            {
                // Create an end message
                var endMsg = (RestUserMessage) await msg.Channel.SendMessageAsync("Ending timer...");
                _endMessages.Add(new TimerEndMessage(endMsg));
                await msg.FinishMessage();
            }

            _timerMessages.RemoveWhere(msg => finishedTimers.Contains(msg));
        }
    }
}