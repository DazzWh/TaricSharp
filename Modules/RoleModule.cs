using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using JetBrains.Annotations;
using TaricSharp.Services;

namespace TaricSharp.Modules
{
    /// <summary>
    /// Module containing commands that affect a users role
    ///
    /// The server is setup so that users have one role to set their name colour.
    /// All other roles are "game roles", that are one reserved colour and can be mentioned.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    [RequireBotPermission(GuildPermission.SendMessages | GuildPermission.ManageRoles)]
    public partial class RoleModule : ModuleBase<SocketCommandContext>
    {
        private readonly LoggingService _loggingService;

        public RoleModule(LoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        private async Task<RestRole> CreateRole(
            string name,
            Color color,
            bool mentionable = false)
        {
            try
            {
                return await Context.Guild.CreateRoleAsync(name, GuildPermissions.None, color, false, mentionable);
            }
            catch(Exception ex)
            {
                await _loggingService.Log(new LogMessage(
                    LogSeverity.Error, 
                    nameof(RoleModule), 
                    $"{ex.Message}"));
                return null;
            }
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
                await _loggingService.Log(new LogMessage(
                    LogSeverity.Error, 
                    nameof(RoleModule), 
                    $"{gameName} role not added to {Context.User.Username}"));
                return null;
            }

            await AddRoleToUser(role);

            return await role;
        }
    }
}