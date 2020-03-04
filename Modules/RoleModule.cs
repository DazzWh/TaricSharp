using System;
using System.Linq;
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
        private readonly Color GameRoleColor = new Color();

        [Command("colour")]
        [Summary("Changes the colour of user to the colour hex")]
        public async Task ColourAsync([Summary("Hexadecimal colour")] string colorStr)
            => await ColorAsync(colorStr);

        [Command("color")]
        [Summary("Changes the colour of user to the colour hex")]
        public async Task ColorAsync([Summary("Hexadecimal colour")] string colorStr)
        {
            if (!ValidHex(colorStr))
            {
                await ReplyAsync($"Incorrect hex code {Context.User.Username}!");
                return;
            }

            await RemoveNonGameColoredRolesFromUser(Context.User);

            var role = CreateRole(Context.User.Username, ColorFromHexString(colorStr));
            if (role.Result == null)
            {
                return;
            }

            await ((SocketGuildUser) Context.User).AddRoleAsync(
                Context.Guild.GetRole(role.Result.Id)
            );
        }

        private bool ValidHex(string hex)
        {
            throw new NotImplementedException();
        }

        private Color ColorFromHexString(string str)
        {
            throw new NotImplementedException();
        }

        private async Task RemoveNonGameColoredRolesFromUser(SocketUser contextUser)
        {
            var colored =
                Context.Guild.Roles.Where(
                    role =>
                        role.Color != GameRoleColor &&
                        role.Color != Color.Default &&
                        role.Members.Contains(Context.User));

            foreach (var role in colored)
            {
                await Context.Guild.GetRole(role.Id).DeleteAsync();
            }
        }

        private async Task<RestRole> CreateRole(string name, Color color)
        {
            return await Context.Guild.CreateRoleAsync(name, null, color);
        }
    }
}
