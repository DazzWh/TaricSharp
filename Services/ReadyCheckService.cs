using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TaricSharp.Services.Games;
using TaricSharp.Messages;

namespace TaricSharp.Services
{
    public class ReadyCheckService
    {
        private readonly DiscordSocketClient _client;
        private readonly GameService _gameService;
        private readonly HashSet<ReadyCheckMessage> _readyChecks;
        private const int ReadyCheckTimeLimitHours = 24; // How many hours a ready check lasts before being deleted

        private readonly Emoji _readyEmoji = new Emoji("✔️");
        private readonly Emoji _notifyEmoji = new Emoji("✉️");
        private readonly Emoji _cancelEmoji = new Emoji("❌");
        private readonly Emoji _finishEmoji = new Emoji("🈵"); // "No Vacancy" in Japanese

        public ReadyCheckService(
            DiscordSocketClient client,
            GameService gameService)
        {
            _client = client;
            _gameService = gameService;
            _readyChecks = new HashSet<ReadyCheckMessage>();
        }

        public void Initialize()
        {
            _client.ReactionAdded += HandleReadyCheckReactionAsync;
        }

        public async Task CreateReadyCheck(
            SocketCommandContext context)
        {
            var msg = await context.Channel.SendMessageAsync("Creating ready check...");

            await msg.AddReactionAsync(_readyEmoji);
            await msg.AddReactionAsync(_notifyEmoji);
            await msg.AddReactionAsync(_cancelEmoji);
            await msg.AddReactionAsync(_finishEmoji);

            var gameInfo = _gameService.GetGameFromMentions(context.Message.MentionedRoles);
            var readyCheck = new ReadyCheckMessage(msg, context.User, gameInfo);
            _readyChecks.Add(readyCheck);
            await readyCheck.AddUser(context.User);

            RemoveOldReadyChecks();
        }

        private void RemoveOldReadyChecks()
        {
            _readyChecks.Where(rc => 
                    (DateTimeOffset.Now - rc.Message.Timestamp).TotalHours > ReadyCheckTimeLimitHours)
                .ToList()
                .ForEach(rc => _readyChecks.Remove(rc));
        }

        private async Task HandleReadyCheckReactionAsync(
            Cacheable<IUserMessage, ulong> message,
            ISocketMessageChannel channel,
            SocketReaction reaction)
        {
            if (!reaction.User.IsSpecified || reaction.User.Value.IsBot)
                return;

            var msg = await channel.GetMessageAsync(message.Id);
            var readyCheck = _readyChecks.FirstOrDefault(m => m.Message.Id == msg.Id);

            if (readyCheck == null)
                return;

            if (reaction.Emote.Equals(_readyEmoji))
                await readyCheck.AddUser(reaction.User.Value);

            if (reaction.Emote.Equals(_cancelEmoji))
                await readyCheck.RemoveUser(reaction.User.Value);

            if (reaction.Emote.Equals(_notifyEmoji))
                await readyCheck.ToggleNotifyOnUser(reaction.User.Value);

            if (reaction.Emote.Equals(_finishEmoji) && reaction.User.Value.Id.Equals(readyCheck.Creator.Id))
            {
                await readyCheck.FinishMessage();
                _readyChecks.Remove(readyCheck);
                return; // FinishMessage removes all reactions so no need to "fall through"
            }

            await readyCheck.Message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
        }
    }
}