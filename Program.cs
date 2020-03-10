using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using TaricSharp.Services;

namespace TaricSharp
{
    internal class Program
    {
        public static Task Main(string[] args)
            => new Program().RunAsync();

        public async Task RunAsync()
        {
            var provider = BuildServiceProvider();
            //provider.GetRequiredService<LoggingService>();    
            await provider.GetRequiredService<StartupService>().StartAsync();
            await Task.Delay(-1);
        }
        private static IServiceProvider BuildServiceProvider() => new ServiceCollection()
            .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose, // Tell the logger to give Verbose amount of info
                MessageCacheSize = 1000 // Cache 1,000 messages per channel
            }))

            .AddSingleton(new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose, // Tell the logger to give Verbose amount of info
                DefaultRunMode = RunMode.Async, // Force all commands to run async by default
            }))
            .AddSingleton<CommandHandler>()
            .AddSingleton<StartupService>()
            .AddSingleton<Random>()
            //.AddSingleton<LoggingService>()
            //.AddSingleton(Configuration);
            .BuildServiceProvider();
    }
}
