using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TaricSharp.Services;

namespace TaricSharp.Modules
{
    /// <summary>
    /// Module that adds the ready check functionality
    /// </summary>
    public class ReadyCheckModule : ModuleBase<SocketCommandContext>
    {
        private readonly ReadyCheckService _readyCheckService;

        public ReadyCheckModule(ReadyCheckService readyCheckService)
        {
            _readyCheckService = readyCheckService;
        }

        [Command("ready")]
        [Alias("check")]
        [Summary("Initiates a ready check")]
        public async Task InitiateReadyCheck()
        {
            // For now just send context, break it down later
            _readyCheckService.CreateReadyCheck(Context);
        }
    }
}