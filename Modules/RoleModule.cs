﻿using System.Threading.Tasks;
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
    [RequireBotPermission(GuildPermission.SendMessages | GuildPermission.ManageRoles)]
    public partial class RoleModule : ModuleBase<SocketCommandContext>
    {
        private readonly Color _gameRoleColor = new Color(0x8787c5);

        private async Task<RestRole> CreateRole(
            string name, 
            Color color, 
            bool mentionable = false)
        {
            var role = Context.Guild.CreateRoleAsync(name, GuildPermissions.None, color);
            
            if (role.Result != null && mentionable)
                await role.Result.ModifyAsync(r => r.Mentionable = true);
            
            return await role;
        }

        private async Task AddRoleToUser(Task<RestRole> role)
        {
            if (Context.User is IGuildUser user)
            {
                await user.AddRoleAsync(role.Result);
            }
        }

        private async Task<RestRole> CreateAndAddRoleToUser(
            string gameName, 
            Color roleColor,
            bool mentionable = false)
        {
            var role = CreateRole(gameName, roleColor, mentionable);
            if (role.Result == null)
            {
                // TODO: log errors here
                return null;
            }

            await AddRoleToUser(role);

            return await role;
        }
    }
}