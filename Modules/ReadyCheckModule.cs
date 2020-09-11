using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using TaricSharp.Services.Games;
using TaricSharp.Services.ReadyCheck;

namespace TaricSharp.Modules
{
    /// <summary>
    /// Module that adds the ready check functionality
    /// </summary>
    public class ReadyCheckModule : ModuleBase<SocketCommandContext>
    {
        private readonly ReadyCheckService _readyCheckService;

        public ReadyCheckModule(
            ReadyCheckService readyCheckService)
        {
            _readyCheckService = readyCheckService;
        }

        [Command("ready")]
        [Alias("check")]
        public async Task InitiateReadyCheck(
            [Remainder] string text)
        {
            await _readyCheckService.CreateReadyCheck(Context, GameFromMentions(Context.Message.MentionedRoles));
        }
        
        // TODO: This is too big to be an enum now. Refactor to a better solution
        private static GameInfo GameFromMentions(
            IEnumerable<SocketRole> messageMentionedRoles)
        {
            return GameInfo.Dota;
        }
    }
}