using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;

namespace TaricSharp.Modules
{
    /// <summary>
    /// Module containing commands that affect a users role
    ///
    /// The server is setup so that users have one role to set their name colour.
    /// All other roles are "game roles", that are one reserved colour and can be mentioned.
    /// </summary>
    public partial class RoleModule : ModuleBase<SocketCommandContext>
    {
        private readonly Color _gameRoleColor = new Color(0x8787c5);

        private async Task<RestRole> CreateRole(string name, Color color)
        {
            return await Context.Guild.CreateRoleAsync(name, GuildPermissions.None, color);
        }

        private async Task AddRoleToUser(Task<RestRole> role)
        {
            if (Context.User is IGuildUser user)
            {
                await user.AddRoleAsync(role.Result);
            }
        }

        private async Task CreateAndAddRoleToUser(string gameName, Color roleColor)
        {
            var role = CreateRole(gameName, roleColor);
            if (role.Result == null)
            {
                // TODO: log errors here
                return;
            }

            await AddRoleToUser(role);
        }
    }
}