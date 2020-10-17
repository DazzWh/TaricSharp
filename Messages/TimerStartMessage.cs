using System;
using Discord;
using Discord.Rest;
using TaricSharp.Services.Games;

namespace TaricSharp.Messages
{
    public class TimerStartMessage : TimerMessage
    {
        private readonly GameInfo _gameInfo;

        public TimerStartMessage(
            RestUserMessage message,
            int minutes,
            GameInfo gameInfo)
            : base(message, minutes)
        {
            _gameInfo = gameInfo;
        }

        protected override EmbedBuilder CountdownMessageEmbedBuilder()
        {
            var timeRemaining = EndTime - DateTime.Now;
            var remainingMessage = timeRemaining.TotalMinutes > 1 ? 
                $"{Math.Round(timeRemaining.TotalMinutes)} minutes." : 
                $"{Math.Round(timeRemaining.TotalSeconds)} seconds.";

            var embed = new EmbedBuilder()
                .WithTitle("⌛ Timer")
                .AddField("Remaining:", remainingMessage)
                .AddField("Users:", $"```{UsersToString()}```", true)
                .WithColor(Color.DarkGreen);
            
            AddGameSpecificEmbedOptions(embed);

            return embed;
        }

        protected override EmbedBuilder FinishedMessageEmbedBuilder()
        {
            return new EmbedBuilder()
                .WithTitle("⌛ Finished")
                .WithColor(Color.DarkRed);
        }
        
        private void AddGameSpecificEmbedOptions(
            EmbedBuilder embed)
        {
            if (_gameInfo != null)
            {
                embed.WithTitle($"{embed.Title} for {_gameInfo.GameName}!")
                    .WithColor(_gameInfo.Color)
                    .WithThumbnailUrl($"{_gameInfo.ImageUrl}");
            }
        }
    }
}