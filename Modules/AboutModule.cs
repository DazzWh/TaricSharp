using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

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
        [Summary("Lists the available commands")]
        public async Task Help()
        {
            var commands = _commandService.Commands;
            var embedBuilder = new EmbedBuilder();

            foreach (var command in commands)
            {
                // Get the command Summary attribute information
                var embedFieldText = command.Summary ?? "No description available\n";

                embedBuilder.AddField(command.Name, embedFieldText);
            }

            await ReplyAsync("Here's a list of commands and their description: ", false, embedBuilder.Build());
        }
    }
}