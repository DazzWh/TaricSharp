using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using TaricSharp.Services.Games;
using TaricSharp.Services.ReadyCheck;

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
        private readonly ReadyCheckService _readyCheckService;
        private readonly GameService _gameService;
        private readonly TimerStartService _timerStartService;
        private readonly TimerEndService _timerEndService;
        private readonly LoggingService _logging;

        public StartupService(
            DiscordSocketClient discord,
            CommandHandler commands,
            PinService pinService,
            ReadyCheckService readyCheckService,
            GameService gameService,
            TimerStartService timerStartService,
            TimerEndService timerEndService,
            LoggingService logging)
        {
            _client = discord;
            _commands = commands;
            _pinService = pinService;
            _readyCheckService = readyCheckService;
            _gameService = gameService;
            _timerStartService = timerStartService;
            _timerEndService = timerEndService;
            _logging = logging;
        }

        public async Task StartAsync()
        {
            _logging.Initialize();
            _readyCheckService.Initialize();
            _gameService.Initialize();
            _timerStartService.Initialize();
            _timerEndService.Initialize();

            await _client.LoginAsync(TokenType.Bot,
                Environment.GetEnvironmentVariable("DiscordToken"));
            await _client.StartAsync();

            await _commands.Initialize();
            _pinService.Initialize();
        }
    }
}