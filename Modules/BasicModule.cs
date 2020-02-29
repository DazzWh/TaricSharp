using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace TaricSharp.Modules
{
    /// <summary>
    /// Module of basic chat commands that can be used by all users
    /// </summary>
    public class BasicModule : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Summary("Returns a pong")]
        public Task PingAsync() => ReplyAsync("pong!");

        [Command("flip")]
        [Summary("Flips a coin")]
        public Task FlipAsync()
        {
            var result = new Random().Next(1, 3) == 1 ? "Heads" : "Tails";
            return ReplyAsync(result);
        }
    }
}
