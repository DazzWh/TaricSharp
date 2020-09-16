using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using TaricSharp.Services;
using TaricSharp.Services.Games;

namespace TaricSharp
{
    internal class Program
    {
        public static Task Main(string[] args)
            => new Program().RunAsync();

        public async Task RunAsync()
        {
            var provider = BuildServiceProvider();
            await provider.GetRequiredService<StartupService>().StartAsync();
            await Task.Delay(-1);
        }
        private static IServiceProvider BuildServiceProvider() => new ServiceCollection()
            .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 1000 
            }))

            .AddSingleton(new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose,
                DefaultRunMode = RunMode.Async
            }))
            .AddSingleton<LoggingService>()
            .AddSingleton<ReadyCheckService>()
            .AddSingleton<GameService>()
            .AddSingleton<TimerStartService>()
            .AddSingleton<TimerEndService>()
            .AddSingleton<CommandHandler>()
            .AddSingleton<PinService>()
            .AddSingleton<Random>()
            .AddSingleton<StartupService>()
            .BuildServiceProvider();
    }
}
