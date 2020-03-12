using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

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
            var gameRole = Context.Guild.Roles
                .FirstOrDefault(role => role.Color == _gameRoleColor &&
                                        role.Name.Equals(gameName, StringComparison.OrdinalIgnoreCase));

            if (gameRole == null)
            {
                await CreateAndAddRoleToUser(gameName, _gameRoleColor);
                await ReplyAsync($"{gameName} role created and added to {Context.User.Username}");
                return;
            }

            if (Context.User is IGuildUser user)
            {
                if (gameRole.Members.Contains(user))
                {
                    await user.RemoveRoleAsync(gameRole);
                    await DeleteEmptyGameRoles();
                    await ReplyAsync($"{gameName} role removed from {Context.User.Username}");
                }
                else
                {
                    await user.AddRoleAsync(gameRole);
                    await ReplyAsync($"{gameName} role added to {Context.User.Username}");
                }
            }
        }

        private async Task DeleteEmptyGameRoles()
        {
            var empty =
                Context.Guild.Roles.Where(
                    role => role.Color == _gameRoleColor &&
                            !role.Members.Any());

            foreach (var role in empty)
            {
                await Context.Guild.GetRole(role.Id).DeleteAsync();
            }
        }
    }
}