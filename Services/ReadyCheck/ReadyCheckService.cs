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
        private HashSet<ReadyCheck> _readyChecks;

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
            //TODO: This doesn't work, and is a mess.
            var msg = await channel.GetMessageAsync(message.Id);
            var readyCheck = _readyChecks.First(m => m.ReadyMsg.Equals(msg));

            if (!IsReadyCheckEmote(reaction) || readyCheck == null || !reaction.User.IsSpecified)
                return;

            var user = reaction.User.Value;

            if (Equals(reaction.Emote, _readyEmoji))
                await readyCheck.AddReadyUser(user);

            if (Equals(reaction.Emote, _cancelEmoji))
                await readyCheck.RemoveReadyUser(user);

            //await msg.RemoveReactionAsync(reaction.Emote, user);

        }

        private bool IsReadyCheckEmote(IReaction reaction)
        {
            var emote = reaction.Emote;
            return Equals(emote, _readyEmoji) ||
                   Equals(emote, _cancelEmoji) ||
                   Equals(emote, _notifyEmoji);
        }
    }
}