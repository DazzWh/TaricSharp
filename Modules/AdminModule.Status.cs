using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace TaricSharp.Modules
{
    /// All Tasks relating to customising the clients status message
    public partial class AdminModule
    {
        [Command("play")]
        [Summary("Sets the bots playing message")]
        public async Task Play(
            [Remainder] [Summary("The game title")]
            string title) =>
            await SetActivity(title, ActivityType.Playing);

        [Command("listen")]
        [Summary("Sets the bots listening message")]
        public async Task Listen(
            [Remainder] [Summary("The song title")]
            string title) =>
            await SetActivity(title, ActivityType.Listening);

        [Command("watch")]
        [Summary("Sets the bots watching message")]
        public async Task Watch(
            [Remainder] [Summary("The movie title")]
            string title) =>
            await SetActivity(title, ActivityType.Watching);

        private async Task SetActivity(string title, ActivityType type)
        {
            await Context.Client.SetActivityAsync(new Game(title, type));
        }

        [Command("play")]
        public async Task Play() => await ClearActivity();

        [Command("listen")]
        public async Task Listen() => await ClearActivity();

        [Command("watch")]
        public async Task Watch() => await ClearActivity();

        private async Task ClearActivity()
        {
            await Context.Client.SetActivityAsync(null);
        }
    }
}