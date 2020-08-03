﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Color = Discord.Color;

namespace TaricSharp.Messages
{
    public class ReadyCheckMessage : UserListMessage
    {
        public readonly IUser Creator;
        private readonly Game _game;

        private readonly Dictionary<ulong, string> _readyUsers;
        private readonly Dictionary<ulong, string> _notifyUsers;

        public ReadyCheckMessage(
            RestUserMessage readyMsg,
            IUser creator,
            Game game) : base(readyMsg)
        {
            Creator = creator;
            _game = game;

            _readyUsers = new Dictionary<ulong, string>();
            _notifyUsers = new Dictionary<ulong, string>();
        }
        
        public override async Task RemoveUser(
            IUser user)
        {
            _readyUsers.Remove(user.Id);
            _notifyUsers.Remove(user.Id);
            await UpdateMessage();
        }

        public async Task ToggleNotifyOnUser(
            IUser user)
        {
            if (_notifyUsers.ContainsKey(user.Id))
            {
                _notifyUsers.Remove(user.Id);
            }
            else
            {
                _readyUsers.TryAdd(user.Id, user.Username);
                _notifyUsers.TryAdd(user.Id, user.Username);
            }

            await UpdateMessage();
        }

        public async Task FinishMessage()
        {
            await UpdateFinishedMessage();

            foreach (var userIdName in _notifyUsers)
            {
                var user = await _message.Channel.GetUserAsync(userIdName.Key);
                await user.SendMessageAsync($"Ready check finished! {_message.GetJumpUrl()}");
            }

            await _message.RemoveAllReactionsAsync();
        }

        protected override async Task UpdateMessage()
        {
            var embed = BaseEmbedBuilder();

            await _message.ModifyAsync(m =>
            {
                m.Content = "";
                m.Embed = embed.Build();
            });
        }

        private async Task UpdateFinishedMessage()
        {
            var embed = BaseEmbedBuilder();

            // Finished specific overrides
            embed.WithTitle("")
                .WithColor(Color.Green)
                .WithFooter("Game on!");

            await _message.ModifyAsync(m =>
            {
                m.Content = "";
                m.Embed = embed.Build();
            });
        }

        private EmbedBuilder BaseEmbedBuilder()
        {
            var embed = new EmbedBuilder()
                .WithAuthor(Creator.Username, Creator.GetAvatarUrl())
                .WithTitle($"Ready check")
                .AddField("Players:", "```" + ReadyUsersToString() + "```", true)
                .WithFooter("Use the reactions to ready up, email will send a pm when people are ready." +
                            Environment.NewLine +
                            "Creator can hit the red \"No Vacancy\" button to conclude the check.");

            AddGameSpecificEmbedOptions(embed);

            return embed;
        }

        private string ReadyUsersToString()
        {
            return _readyUsers.Count > 0
                ? _readyUsers.Aggregate("",
                    (current, user) =>
                        current + Environment.NewLine +
                        user.Value + " " + NotifyIconIfInNotifyList(user.Key))
                : "--None--";
        }

        private string NotifyIconIfInNotifyList(
            ulong userId)
        {
            return _notifyUsers.ContainsKey(userId) ? "✉️" : "";
        }

        private void AddGameSpecificEmbedOptions(
            EmbedBuilder embed)
        {
            // TODO: Refactor how games data is handled. This is messy.
            switch (_game)
            {
                case Game.ProjectWinter:
                    embed.WithTitle($"{embed.Title} for Project Winter!")
                        //.WithUrl("steam://run/774861")
                        .WithColor(Color.Blue)
                        .WithThumbnailUrl("https://steamcdn-a.akamaihd.net/steam/apps/774861/header.jpg");
                    break;

                case Game.Dota:
                    embed.WithTitle($"{embed.Title} for Dota!")
                        //.WithUrl("steam://run/570")
                        .WithColor(Color.DarkRed)
                        .WithThumbnailUrl("https://steamcdn-a.akamaihd.net/steam/apps/570/header.jpg");
                    break;

                case Game.FallGuys:
                    embed.WithTitle($"{embed.Title} for Fall Guys!")
                        .WithColor(Color.Magenta)
                        .WithThumbnailUrl("https://steamcdn-a.akamaihd.net/steam/apps/1097150/header.jpg");
                    break;

                case Game.Pavlov:
                    embed.WithTitle($"{embed.Title} for Pavlov!")
                        .WithColor(Color.DarkGreen)
                        .WithThumbnailUrl("https://steamcdn-a.akamaihd.net/steam/apps/555160/header.jpg");
                    break;

                case Game.KillingFloor:
                    embed.WithTitle($"{embed.Title} for Killing Floor!")
                        .WithColor(Color.Red)
                        .WithThumbnailUrl("https://steamcdn-a.akamaihd.net/steam/apps/232090/header.jpg");
                    break;

                case Game.JackBox:
                    embed.WithTitle($"{embed.Title} for JackBox!")
                        .WithColor(Color.LightOrange)
                        .WithThumbnailUrl("https://steamcdn-a.akamaihd.net/steam/apps/331670/header.jpg");
                    break;

                case Game.None:
                    embed.WithTitle($"{embed.Title}!");
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override int GetHashCode() => _message.GetHashCode();
    }
}