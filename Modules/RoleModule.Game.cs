using System.Threading.Tasks;
using Discord.Commands;

namespace TaricSharp.Modules
{
    public partial class RoleModule
    {
        [Command("game")]
        [Summary("Adds or removes a GameRole to user")]
        public async Task GameAsync() => await ReplyAsync("game command!");
    }
}