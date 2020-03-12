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
        private readonly Random _rnd;

        public BasicModule(Random rnd)
        {
            _rnd = rnd;
        }

        [Command("ping")]
        [Summary("Returns a pong")]
        public Task PingAsync() => ReplyAsync("pong!");

        [Command("flip")]
        [Summary("Flips a coin")]
        public Task FlipAsync()
        {
            var result = _rnd.Next(1, 3) == 1 ? "Heads" : "Tails";
            return ReplyAsync($"{Context.User.Username} rolled {result}!");
        }

        [Command("roll")]
        [Summary("Rolls a dice between 1 and 100")]
        [Remarks("Also accepts up to two numbers after as min or max values")]
        public Task RollAsync()
            => RollReply(_rnd.Next(101));

        [Command("roll")]
        [Summary("Rolls a dice between 0 and a max number")]
        public Task RollAsync(
            [Summary("The max number to roll")] int max)
            => RollReply(_rnd.Next(max));

        [Command("roll")]
        [Summary("Rolls a dice between min and max number")]
        public Task RollAsync(
            [Summary("The min number to roll")] int min,
            [Summary("The max number to roll")] int max)
            => RollReply(_rnd.Next(min, max));

        private Task<IUserMessage> RollReply(int num)
            => ReplyAsync($"{Context.User.Username} rolled {num}!");
    }
}
