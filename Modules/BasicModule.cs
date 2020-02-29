using Discord;
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
            return ReplyAsync($"{Context.User.Username} rolled {result}!");
        }

        [Command("roll")]
        [Summary("Rolls a dice between 1 and 100")]
        public Task RollAsync() => RollReply(new Random().Next(101));

        [Command("roll")]
        [Summary("Rolls a dice between 0 and a max number")]
        public Task RollAsync(
            [Summary("The max number to roll")] int max) => RollReply(new Random().Next(max));

        [Command("roll")]
        [Summary("Rolls a dice between 0 and a max number")]
        public Task RollAsync(
            [Summary("The min number to roll")] int min,
            [Summary("The min number to roll")] int max)
            => RollReply(new Random().Next(min, max));

        private Task<IUserMessage> RollReply(int num) => ReplyAsync($"{Context.User.Username} rolled {num}!");
    }
}
