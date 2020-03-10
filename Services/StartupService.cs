using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace TaricSharp.Services
{
    /// <summary>
    /// A service to start the bot and initialize other dependencies
    /// </summary>
    public class StartupService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandHandler _commands;
        private readonly IServiceProvider _service;

        public StartupService(
            DiscordSocketClient discord,
            CommandHandler commands,
            IServiceProvider service)
        {
            _discord = discord;
            _commands = commands;
            _service = service;
        }

        public async Task StartAsync()
        {
            await _discord.LoginAsync(TokenType.Bot, 
                Environment.GetEnvironmentVariable("DiscordToken"));     
            await _discord.StartAsync();

            await _commands.Initialize();
        }
    }
}