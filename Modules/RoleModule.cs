using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;

namespace TaricSharp.Modules
{
    /// <summary>
    /// Module containing commands that affect a users role
    /// </summary>
    public class RoleModule : ModuleBase<SocketCommandContext>
    {
        private readonly Color GameRoleColor = new Color();

        [Command("color")]
        [Summary("Changes the colour of user to the colour hex")]
        public async Task ColorAsync([Summary("Hexadecimal colour")] string colour)
            => await ColourAsync(colour);

        [Command("colour")]
        [Summary("Changes the colour of user to the colour hex")]
        public async Task ColourAsync([Summary("Hexadecimal colour")] string colour)
        {
            // Users only have 1 colour role that isn't a GameRole
            var userRole = Context.Guild.Roles.SingleOrDefault(
                role => role.Color != GameRoleColor &&
                        role.Color != Color.Default &&
                        role.Members.Contains(Context.User));
        }
    }
}
