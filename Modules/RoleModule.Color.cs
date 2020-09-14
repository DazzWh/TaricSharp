using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Discord.WebSocket;
using TaricSharp.Extensions;

namespace TaricSharp.Modules
{
    public partial class RoleModule
    {
        [Command("color")]
        [Alias("colour")]
        [Summary("Changes the colour of user to the colour hex")]
        [Remarks("Accepts the format like #c55fc5, google color picker and copy the HEX")]
        public async Task ColorAsync(
            [Summary("Hexadecimal colour")] string colorStr)
        {
            if (!colorStr.IsValidHexString())
            {
                await ReplyAsync($"Invalid hex code {Context.User.Username}");
                return;
            }

            if (colorStr.ToColor() == _gameRoleColor)
            {
                await ReplyAsync($"Sorry, that colour is reserved for GameRoles {Context.User.Username}");
                return;
            }

            await RemoveNonGameColoredRolesFromUser(Context.User);

            var role = CreateAndAddRoleToUser(Context.User.Username, colorStr.ToColor());
            if (role.Result == null)
            {
                // TODO: log errors here
                return;
            }

            await role.Result.ModifyAsync(x =>
                x.Position = Context.Guild.Roles.Count(r => r.Color == _gameRoleColor) + 1);
        }

        private async Task RemoveNonGameColoredRolesFromUser(SocketUser contextUser)
        {
            var colored =
                Context.Guild.Roles.Where(
                    role =>
                        role.Color != _gameRoleColor &&
                        role.Color != Color.Default &&
                        role.Members.Contains(contextUser));

            foreach (var role in colored)
            {
                await Context.Guild.GetRole(role.Id).DeleteAsync();
            }
        }
    }
}
