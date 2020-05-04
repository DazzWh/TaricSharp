using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace TaricSharp.Services
{
    public class ReadyCheckService
    {
        private readonly DiscordSocketClient _client;
        private HashSet<ReadyCheck> _readyChecks;

        private Emote _readyEmote;
        private Emote _notifyEmote;
        private Emote _cancelEmote;

        public ReadyCheckService(DiscordSocketClient client)
        {
            _client = client;
            _readyChecks = new HashSet<ReadyCheck>();
        }

        public void Initialize()
        {
            _client.ReactionAdded += HandleReadyCheckReactionAsync;
        }

        public void CreateReadyCheck(SocketCommandContext context)
        {
            // Create an embed message to be the ready check

            // Add reactions to that message

            // Add that message to the list of ReadyChecks
        }

        private static async Task HandleReadyCheckReactionAsync(
            Cacheable<IUserMessage, ulong> message,
            ISocketMessageChannel channel,
            SocketReaction reaction)
        {
            // Check if 
        }
    }
}