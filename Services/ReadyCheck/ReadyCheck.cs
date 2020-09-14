using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using TaricSharp.Extensions;
using TaricSharp.Services.Games;
using Color = Discord.Color;

namespace TaricSharp.Services.ReadyCheck
{
    public class ReadyCheck
    {
        public readonly RestUserMessage ReadyMsg;
        public readonly IUser Creator;
        private readonly GameInfo _gameInfo;

        private readonly Dictionary<ulong, string> _readyUsers;
        private readonly Dictionary<ulong, string> _notifyUsers;

        public ReadyCheck(
            RestUserMessage readyMsg,
            IUser creator,
            GameInfo gameInfo)
        {
            ReadyMsg = readyMsg;
            Creator = creator;
            _gameInfo = gameInfo;

            _readyUsers = new Dictionary<ulong, string>();
            _notifyUsers = new Dictionary<ulong, string>();
        }

        public async Task AddReadyUser(
            IUser user)
        {
            _readyUsers.TryAdd(user.Id, user.Username);
            await UpdateMessage();
        }

        public async Task RemoveReadyUser(
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

        public async Task Finish()
        {
            await UpdateFinishedMessage();

            foreach (var userIdName in _notifyUsers)
            {
                var user = await ReadyMsg.Channel.GetUserAsync(userIdName.Key);
                await user.SendMessageAsync($"Ready check finished! {ReadyMsg.GetJumpUrl()}");
            }

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
                .WithTitle("Ready check")
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
            if (_gameInfo != null)
            {
                embed.WithTitle($"{embed.Title} for {_gameInfo.GameName}!")
                    .WithColor(_gameInfo.Color)
                    .WithThumbnailUrl($"{_gameInfo.ImageUrl}");
            }
        }
    }
}