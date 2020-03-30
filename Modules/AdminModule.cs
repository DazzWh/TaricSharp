using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace TaricSharp.Modules
{
    /// <summary>
    /// Module that has commands that can only be used by the server owner
    /// </summary>
    [RequireOwner]
    [RequireBotPermission(GuildPermission.SendMessages)]
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        [Command("say")]
        [Summary("Repeats message into given channel")]
        public async Task Say(
            [Summary("The channel to repeat a message into")] ISocketMessageChannel channel,
            [Remainder] [Summary("The message")] string message)
        {
            await channel.SendMessageAsync(message);
        }
        
    }
}