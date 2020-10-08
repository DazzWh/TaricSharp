using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using TaricSharp.Extensions;
using TaricSharp.Messages;

namespace TaricSharp.Services.Timer
{
    public abstract class TimerService
    {
        protected readonly HashSet<UserListMessage> Messages = new HashSet<UserListMessage>();
        protected readonly Emoji AcceptEmoji = new Emoji("✔️");
        private readonly DiscordSocketClient _client;
        private const int CheckTimeInSeconds = 5; // How often we update messages
        private System.Threading.Timer _timer;
        
        protected abstract Task OnMessageComplete(TimerMessage msg);

        protected TimerService(DiscordSocketClient client)
        {
            _client = client;
        }
        
        public void Initialize()
        {
            _client.ReactionAdded += HandleReactionsAsync;

            _timer = new System.Threading.Timer(async e => await CheckMessages(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(CheckTimeInSeconds));
        }
        
        private async Task CheckMessages()
        {
            foreach (var msg in Messages)
            {
                await msg.UpdateMessage();
            }

            await CheckForCompletedTimers();
        }
        
        private async Task CheckForCompletedTimers()
        {
            foreach (var msg in 
                Messages
                    .Cast<TimerMessage>()
                    .Where(msg => msg.EndTime < DateTime.Now)
                    .ToArray())
            {
                await msg.UpdateMessage();
                await OnMessageComplete(msg);
                await msg.Message.RemoveAllReactionsAsync();
                Messages.Remove(msg);
            }
        }
        
        private async Task HandleReactionsAsync(
            Cacheable<IUserMessage, ulong> message,
            ISocketMessageChannel channel,
            SocketReaction reaction)
        {
            var timerMessage = Messages.FirstOrDefault(m => m.Id == message.Id);
            if (reaction.UserNullOrBot() || timerMessage == null)
                return;
            
            if (reaction.Emote.Equals(AcceptEmoji))
                await timerMessage.AddUser(reaction.User.Value);
            
            await timerMessage.Message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
        }
    }
}