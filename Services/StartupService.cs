﻿using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace TaricSharp.Services
{
    /// <summary>
    /// A service to start the bot and initialize other dependencies
    /// </summary>
    public class StartupService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandHandler _commands;
        private readonly PinService _pinService;
        private readonly LoggingService _logging;

        public StartupService(
            DiscordSocketClient discord,
            CommandHandler commands,
            PinService pinService,
            LoggingService logging)
        {
            _client = discord;
            _commands = commands;
            _pinService = pinService;
            _logging = logging;
        }

        public async Task StartAsync()
        {
            _logging.Initialize();

            await _client.LoginAsync(TokenType.Bot,
                Environment.GetEnvironmentVariable("DiscordToken"));
            await _client.StartAsync();

            await _commands.Initialize();
            _pinService.Initialize();
        }
    }
}