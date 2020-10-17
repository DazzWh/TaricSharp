using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using JetBrains.Annotations;

namespace TaricSharp.Modules
{
    /// <summary>
    /// Module that has commands that can only be used by the server owner
    /// </summary>
    [RequireOwner]
    [RequireBotPermission(GuildPermission.SendMessages)]
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public partial class AdminModule : ModuleBase<SocketCommandContext>
    {
        [Command("say")]
        [Summary("Repeats message into given channel")]
        public async Task Say(
            [Summary("The channel to repeat a message into")]
            ISocketMessageChannel channel,
            [Remainder] [Summary("The message")] string message)
        {
            await channel.SendMessageAsync(message);
        }
    }
}