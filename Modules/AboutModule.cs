using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using MoreLinq.Extensions;

namespace TaricSharp.Modules
{

    /// <summary>
    /// Module that provides information about the bot
    /// </summary>
    public class AboutModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commandService;

        public AboutModule(CommandService commandService)
        {
            _commandService = commandService;
        }

        [Command("help")]
        [Alias("commands")]
        [Summary("Lists the available commands")]
        public async Task Help()
        {
            var commands = _commandService.Commands.DistinctBy(cmd => cmd.Name);
            var embedBuilder = new EmbedBuilder
            {
                Title = "Command list",
                Color = new Color(0xc55fc5)
            };

            foreach (var command in commands)
            {
                var sb = new StringBuilder();

                sb.AppendLine(command.Summary ?? "No description available");
                sb.AppendLine(command.Remarks ?? string.Empty);

                embedBuilder.AddField(command.Name, sb.ToString());
            }

            await ReplyAsync(null, false, embedBuilder.Build());
        }
    }
}