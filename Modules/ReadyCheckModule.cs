using System.Threading.Tasks;
using Discord.Commands;
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
        private readonly GameService _gameService;

        public ReadyCheckModule(
            ReadyCheckService readyCheckService,
            GameService gameService)
        {
            _readyCheckService = readyCheckService;
            _gameService = gameService;
        }
        
        [Command("ready")]
        [Alias("check")]
        [Summary("Initiates a ready check")]
        [Remarks("Searches message for GameRole mentions to set the ReadyCheck theme")]
        public async Task InitiateReadyCheck()
        {
            await _readyCheckService.CreateReadyCheck(Context, GameInfo.None);
        }

        [Command("ready")]
        [Alias("check")]
        public async Task InitiateReadyCheck(
            [Remainder] string text)
        {
            var gameInfo = _gameService.GetGameFromMentions(Context.Message.MentionedRoles);
            await _readyCheckService.CreateReadyCheck(Context, gameInfo);
        }
        
    }
}