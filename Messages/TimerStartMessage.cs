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
            var embed = new EmbedBuilder()
                .WithTitle($"⌛ Timer")
                .AddField("Remaining:", $"{Math.Round((EndTime - DateTime.Now).TotalMinutes)} minutes.")
                .AddField("Committed:", $"```{UsersToString()}```", true)
                .WithColor(Color.DarkGreen);

            return embed;
        }

        protected override EmbedBuilder FinishedMessageEmbedBuilder()
        {
            return new EmbedBuilder()
                .WithTitle($"Timer finished")
                .AddField("Committed:", $"```{UsersToString()}```", true)
                .WithColor(Color.DarkRed);
        }
    }
}