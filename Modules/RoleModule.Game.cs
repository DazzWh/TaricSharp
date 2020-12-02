using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TaricSharp.Extensions;

namespace TaricSharp.Modules
{
    public partial class RoleModule
    {
        [Command("game")]
        [Summary("Adds or removes a GameRole to user")]
        [Remarks("Just type \"!game name of game\"")]
        public async Task GameAsync(
            [Summary("Game name")] [Remainder] string gameName)
        {
            // Old versions had commands as `game add` or `game remove` instead of `game`
            // Strip "add " or "remove " from game name
            gameName = gameName.TrimStart("add ").TrimStart("remove ");

            var gameRole = Context.Guild.Roles
                .FirstOrDefault(role => role.Color == Constants.GameRoleColor &&
                                        role.Name.Equals(gameName, StringComparison.OrdinalIgnoreCase));

            if (gameRole == null)
            {
                await CreateAndAddRoleToUser(gameName, Constants.GameRoleColor, true);
                await ReplyAsync($"{gameName} role created and added to {Context.User.Username}");
                return;
            }

            if (Context.User is IGuildUser user)
            {
                if (gameRole.Members.Contains(user))
                {
                    var roleName = gameRole.Name;
                    await user.RemoveRoleAsync(gameRole);
                    await DeleteEmptyGameRoles();
                    await ReplyAsync($"{roleName} role removed from {Context.User.Username}");
                }
                else
                {
                    await user.AddRoleAsync(gameRole);
                    await ReplyAsync($"{gameRole.Name} role added to {Context.User.Username}");
                }
            }
        }

        private async Task DeleteEmptyGameRoles()
        {
            var empty =
                Context.Guild.Roles.Where(
                    role => role.Color == Constants.GameRoleColor &&
                            !role.Members.Any());

            foreach (var role in empty)
            {
                await Context.Guild.GetRole(role.Id).DeleteAsync();
            }
        }
    }
}