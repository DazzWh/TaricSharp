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
        public async Task GameAsync(
            [Summary("Game name")] string gameName)
        {
            var gameRole = Context.Guild.Roles
                .FirstOrDefault(role => role.Color == _gameRoleColor &&
                                        role.Name.Equals(gameName, StringComparison.OrdinalIgnoreCase));

            if (gameRole == null)
            {
                await CreateAndAddRoleToUser(gameName, _gameRoleColor);
                await ReplyAsync($"{gameName} created and added to {Context.User.Username}");
                return;
            }

            if (Context.User is IGuildUser user)
            {
                if (gameRole.Members.Contains(user))
                {
                    await user.RemoveRoleAsync(gameRole);
                    await ReplyAsync($"{gameName} removed from {Context.User.Username}");
                }
                else
                {
                    await user.AddRoleAsync(gameRole);
                    await ReplyAsync($"{gameName} added to {Context.User.Username}");
                }
            }
        }
    }
}