using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace TaricSharp
{
    internal class Program
    {
        private DiscordSocketClient _client;
        private CommandHandler _commandHandler;

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _client.Log += Log;

            _commandHandler = new CommandHandler(_client, new CommandService());
            await _commandHandler.InstallCommandsAsync();

            await _client.LoginAsync(TokenType.Bot,
                Environment.GetEnvironmentVariable("DiscordToken"));
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
