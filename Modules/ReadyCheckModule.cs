using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using TaricSharp.Services.ReadyCheck;
using Game = TaricSharp.Services.ReadyCheck.Game;

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
        [Summary("Initiates a ready check")]
        [Remarks("Searches message for GameRole mentions to set the ReadyCheck theme")]
        public async Task InitiateReadyCheck()
        {
            await _readyCheckService.CreateReadyCheck(Context, Game.None);
        }

        [Command("ready")]
        [Alias("check")]
        public async Task InitiateReadyCheck(
            [Remainder] string text)
        {
            await _readyCheckService.CreateReadyCheck(Context, GameFromMentions(Context.Message.MentionedRoles));
        }

        private static Game GameFromMentions(
            IEnumerable<SocketRole> messageMentionedRoles)
        {
            foreach (var role in messageMentionedRoles)
            {
                if (role.Name.Equals("Dota"))
                    return Game.Dota;

                if (role.Name.Equals("Winter"))
                    return Game.ProjectWinter;

                if (role.Name.Equals("Fall Guys"))
                    return Game.FallGuys;

                if (role.Name.Equals("Pavlov"))
                    return Game.Pavlov;

                if (role.Name.Equals("KF2"))
                    return Game.KillingFloor;

                if (role.Name.Equals("Jackbox"))
                    return Game.JackBox;
            }

            return Game.None;
        }
    }
}