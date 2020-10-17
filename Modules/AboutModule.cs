using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using JetBrains.Annotations;
using MoreLinq.Extensions;

namespace TaricSharp.Modules
{
    /// <summary>
    /// Module that provides information about the bot
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    [RequireBotPermission(GuildPermission.SendMessages)]
    public class AboutModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commandService;

        public AboutModule(CommandService commandService)
        {
            _commandService = commandService;
        }

        [Command("about")]
        [Summary("Displays tech info")]
        public async Task About()
        {
            var embedBuilder = new EmbedBuilder
            {
                Color = new Color(0x31bdc3),
                ImageUrl = "https://cdn.discordapp.com/app-icons/186101048027512832/98d08bcd400ee36c1ab1d6a734fb92b9.png?size=256"
            };
            
            var os = Environment.OSVersion;
            embedBuilder.AddField("My Home", 
                $"Platform: {os.Platform} \n Version: {os.VersionString}");

            embedBuilder.AddField("My Brain",
                "Code publicly hosted on [Github](https://github.com/DazzWh/TaricSharp), pull requests welcome!");

            await ReplyAsync(null, false, embedBuilder.Build());
        }

        [Command("help")]
        [Alias("commands")]
        [Summary("Lists the available commands")]
        public async Task Help()
        {
            var commands = _commandService.Commands
                .Where(cmd => !cmd.Module.Preconditions.Contains(new RequireOwnerAttribute()))
                .DistinctBy(cmd => cmd.Name);

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