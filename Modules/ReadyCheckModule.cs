using System.Threading.Tasks;
using Discord.Commands;
using JetBrains.Annotations;
using TaricSharp.Services;

namespace TaricSharp.Modules
{
    /// <summary>
    /// Module that adds the ready check functionality
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
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
        [Summary("Initiates a ready check")]
        [Remarks("Searches message for GameRole mentions to set the ReadyCheckMessage theme")]
        public async Task InitiateReadyCheck()
        {
            await _readyCheckService.CreateReadyCheck(Context);
        }
        
        [Command("ready")]
        [Alias("check")]
        public async Task InitiateReadyCheck(
            [Remainder] string text)
        {
            await _readyCheckService.CreateReadyCheck(Context);
        }
    }
}