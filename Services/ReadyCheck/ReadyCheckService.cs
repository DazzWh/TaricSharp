using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;

namespace TaricSharp.Services
{
    public class ReadyCheckService
    {
        private readonly DiscordSocketClient _client;
        private readonly HashSet<ReadyCheck> _readyChecks;

        private readonly Emoji _readyEmoji = new Emoji("✔️");
        private readonly Emoji _notifyEmoji =  new Emoji("✉️");
        private readonly Emoji _cancelEmoji =  new Emoji("❌");

        public ReadyCheckService(DiscordSocketClient client)
        {
            _client = client;
            _readyChecks = new HashSet<ReadyCheck>();
        }

        public void Initialize()
        {
            _client.ReactionAdded += HandleReadyCheckReactionAsync;
        }

        public async Task CreateReadyCheck(SocketCommandContext context)
        {
            // Create an embed message to be the ready check
            var msg = await context.Channel.SendMessageAsync("Creating ready check...");

            // Add reactions to that message to act as buttons
            await msg.AddReactionAsync(_readyEmoji);
            await msg.AddReactionAsync(_notifyEmoji);
            await msg.AddReactionAsync(_cancelEmoji);

            // Add that message to the list of ReadyChecks
            var readyCheck = new ReadyCheck(msg, 2);
            _readyChecks.Add(readyCheck);
            await readyCheck.AddReadyUser(context.User);
        }

        private async Task HandleReadyCheckReactionAsync(
            Cacheable<IUserMessage, ulong> message,
            ISocketMessageChannel channel,
            SocketReaction reaction)
        {
            if (!reaction.User.IsSpecified || reaction.User.Value.IsBot)
                return;

            //TODO: Check potential rate limiting issues
            var msg = await channel.GetMessageAsync(message.Id);
            var readyCheck = _readyChecks.FirstOrDefault(m => m.ReadyMsg.Id == msg.Id);

            if (readyCheck == null || !reaction.User.IsSpecified)
                return;

            if (reaction.Emote.Equals(_readyEmoji))
                await readyCheck.AddReadyUser(reaction.User.Value);

            if (reaction.Emote.Equals(_cancelEmoji))
                await readyCheck.RemoveReadyUser(reaction.User.Value);

            if (reaction.Emote.Equals(_notifyEmoji))
                await readyCheck.ToggleNotifyOnUser(reaction.User.Value);

            await readyCheck.ReadyMsg.RemoveReactionAsync(reaction.Emote, reaction.User.Value);

        }
    }
}