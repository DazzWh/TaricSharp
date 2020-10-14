using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace TaricSharp.Modules
{
    /// All Tasks relating to customising the clients status message
    public partial class AdminModule
    {
        [Command("status")]
        [Summary("Sets the bots status message")]
        public async Task Status(
            [Remainder] [Summary("The status message")]
            string title) =>
            await Context.Client.SetActivityAsync(new Game(title, ActivityType.CustomStatus));

        [Command("play")]
        [Summary("Sets the bots playing message")]
        public async Task Play(
            [Remainder] [Summary("The game title")]
            string title) =>
            await Context.Client.SetGameAsync(title);

        [Command("listen")]
        [Summary("Sets the bots listening message")]
        public async Task Listen(
            [Remainder] [Summary("The song title")]
            string title) =>
            await Context.Client.SetActivityAsync(new Game(title, ActivityType.Listening));

        [Command("watch")]
        [Summary("Sets the bots watching message")]
        public async Task Watch(
            [Remainder] [Summary("The movie title")]
            string title) =>
            await Context.Client.SetActivityAsync(new Game(title, ActivityType.Watching));
    }
}