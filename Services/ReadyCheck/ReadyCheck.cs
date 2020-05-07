﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using MoreLinq.Extensions;
using Color = Discord.Color;

namespace TaricSharp.Services.ReadyCheck
{
    public class ReadyCheck
    {
        public readonly RestUserMessage ReadyMsg;
        public readonly IUser Creator;
        private readonly Game _game;

        private readonly HashSet<IUser> _readyUsers;
        private readonly HashSet<IUser> _notifyUsers;

        public ReadyCheck(
            RestUserMessage readyMsg,
            IUser creator,
            Game game)
        {
            ReadyMsg = readyMsg;
            Creator = creator;
            _game = game;

            _readyUsers = new HashSet<IUser>();
            _notifyUsers = new HashSet<IUser>();
        }

        public async Task AddReadyUser(
            IUser user)
        {
            _readyUsers.Add(user);
            await UpdateMessage();
        }

        public async Task RemoveReadyUser(
            IUser user)
        {
            _readyUsers.Remove(user);
            _notifyUsers.Remove(user);
            await UpdateMessage();
        }

        public async Task ToggleNotifyOnUser(
            IUser user)
        {
            if (_notifyUsers.Contains(user))
            {
                _notifyUsers.Remove(user);
            }
            else
            {
                _readyUsers.Add(user);
                _notifyUsers.Add(user);
            }

            await UpdateMessage();
        }

        public async Task Finish()
        {
            await UpdateFinishedMessage();

            _notifyUsers.ForEach(u =>
                u.SendMessageAsync($"Ready check finished! {ReadyMsg.GetJumpUrl()}"));

            await ReadyMsg.RemoveAllReactionsAsync();
        }

        private async Task UpdateMessage()
        {
            var embed = BaseEmbedBuilder();

            await ReadyMsg.ModifyAsync(m =>
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

            await ReadyMsg.ModifyAsync(m =>
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
                        user.Username + " " + NotifyIconIfInNotifyList(user))
                : "--None--";
        }

        private string NotifyIconIfInNotifyList(
            IUser user)
        {
            return _notifyUsers.Contains(user) ? "✉️" : "";
        }

        private void AddGameSpecificEmbedOptions(
            EmbedBuilder embed)
        {
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

                case Game.None:
                    embed.WithTitle($"{embed.Title}!");
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}