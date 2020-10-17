using System;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;

namespace TaricSharp.Messages
{
    public class TimerStartMessage : TimerMessage
    {
        public TimerStartMessage(
            RestUserMessage message,
            int minutes) 
            : base(message, minutes){}

        protected override EmbedBuilder CountdownMessageEmbedBuilder()
        {
            var timeRemaining = (EndTime - DateTime.Now);
            var remainingMessage = timeRemaining.TotalMinutes > 1 ? 
                $"{Math.Round(timeRemaining.TotalMinutes)} minutes." : 
                $"{Math.Round(timeRemaining.TotalSeconds)} seconds.";

            var embed = new EmbedBuilder()
                .WithTitle($"⌛ Timer")
                .AddField("Remaining:", remainingMessage)
                .AddField("Users:", $"```{UsersToString()}```", true)
                .WithColor(Color.DarkGreen);

            return embed;
        }

        protected override EmbedBuilder FinishedMessageEmbedBuilder()
        {
            return new EmbedBuilder()
                .WithTitle($"⌛ Finished")
                .WithColor(Color.DarkRed);
        }
    }
}