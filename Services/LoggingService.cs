using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TaricSharp.Modules;
using TaricSharp.Services.Games;

namespace TaricSharp.Services
{
    public class LoggingService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly GameService _gameService;
        private string _logDirectory { get; }
        private string _logFile => Path.Combine(_logDirectory, $"{DateTime.UtcNow:yyyy-MM-dd}.txt");

        public LoggingService(
            DiscordSocketClient client, 
            CommandService commands,
            GameService gameService)
        {
            _client = client;
            _commands = commands;
            _gameService = gameService;
            _logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
        }
        
        public void Initialize()
        {
            _client.Log += Log;
            _commands.Log += Log;
            _gameService.Log += Log;
        }

        public Task Log(LogMessage msg)
        {
            if (!Directory.Exists(_logDirectory))
                Directory.CreateDirectory(_logDirectory);

            if (!File.Exists(_logFile))
                File.Create(_logFile).Dispose();

            var logText = $"{DateTime.UtcNow:hh:mm:ss} [{msg.Severity}] {msg.Source}: {msg.Exception?.ToString() ?? msg.Message}";
            File.AppendAllText(_logFile, logText + "\n");

            return Console.Out.WriteLineAsync(logText);
        }

    }
}