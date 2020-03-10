using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace TaricSharp.Modules
{
    /// <summary>
    /// Module containing commands that affect a users role
    ///
    /// The server is setup so that users have one role to set their name colour.
    /// All other roles are "game roles", that are one reserved colour and can be mentioned.
    /// </summary>
    public class RoleModule : ModuleBase<SocketCommandContext>
    {
        private readonly Color _gameRoleColor = new Color(0x8787c5);

        [Command("color")]
        [Alias("Colour")]
        [Summary("Changes the colour of user to the colour hex")]
        public async Task ColorAsync(
            [Summary("Hexadecimal colour")] string colorStr)
        {
            if (!ValidHexString(colorStr))
            {
                await ReplyAsync($"Incorrect hex code {Context.User.Username}");
                return;
            }
            
            if (ColorFromHexString(colorStr) == _gameRoleColor)
            {
                await ReplyAsync($"Sorry, that colour is reserved for GameRoles {Context.User.Username}");
                return;
            }

            await RemoveNonGameColoredRolesFromUser(Context.User);

            var role = CreateRole(Context.User.Username, ColorFromHexString(colorStr));
            if (role.Result == null)
            {
                // TODO: log errors here
                return;
            }

            await role.Result.ModifyAsync(x =>
                x.Position = Context.Guild.Roles.Count(r => r.Color == _gameRoleColor) + 1);

            if (Context.User is IGuildUser user)
            {
                await user.AddRoleAsync(role.Result);
            }
        }

        private bool ValidHexString(string str)
        {
            var rx = new Regex(@"^#?[A-Fa-f0-9]{6}$");
            return rx.IsMatch(str);

        }

        private Color ColorFromHexString(string str)
        {
            if (str.StartsWith("#"))
                str = str.Substring(1);

            return new Color(
                Convert.ToInt32(str.Substring(0, 2), 16),
                Convert.ToInt32(str.Substring(2, 2), 16),
                Convert.ToInt32(str.Substring(4, 2), 16)
            );
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

        private async Task<RestRole> CreateRole(string name, Color color)
        {
            return await Context.Guild.CreateRoleAsync(name, GuildPermissions.None, color);
        }
    }
}
