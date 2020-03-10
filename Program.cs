using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TaricSharp.Services;

namespace TaricSharp
{
    internal class Program
    {
        public static Task Main(string[] args)
            => Initialize.RunAsync(args);
    }
}
