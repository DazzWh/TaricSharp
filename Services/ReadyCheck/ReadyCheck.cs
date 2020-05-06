using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Color = Discord.Color;

namespace TaricSharp.Services.ReadyCheck
{
    public class ReadyCheck
    {
        public readonly RestUserMessage ReadyMsg;
        public readonly IUser Creator;
        private readonly HashSet<IUser> _readyUsers;
        private readonly Game _game;

        public ReadyCheck(
            RestUserMessage readyMsg,
            IUser creator,
            Game game)
        {
            ReadyMsg = readyMsg;
            Creator = creator;
            _game = game;
            _readyUsers = new HashSet<IUser>();
        }

        public async Task AddReadyUser(IUser user)
        {
            if (_readyUsers.Contains(user))
                return;
            
            _readyUsers.Add(user);
            await UpdateMessage();
        }

        public async Task RemoveReadyUser(IUser user)
        {
            if (!_readyUsers.Contains(user))
                return;

            _readyUsers.Remove(user);
            await UpdateMessage();
        }

        public async Task ToggleNotifyOnUser(IUser user)
        {
            throw new NotImplementedException();
        }

        public async Task Finish()
        {
            await UpdateFinishedMessage();
            
            //TODO: Ping all users set to notify when implemented

            await ReadyMsg.RemoveAllReactionsAsync();
        }

        private async Task UpdateMessage()
        {
            var embed = GenerateMessageEmbed();

            await ReadyMsg.ModifyAsync(m =>
            {
                m.Content = "";
                m.Embed = embed.Build();
            });
        }

        private async Task UpdateFinishedMessage()
        {
            var embed = GenerateMessageEmbed();

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

        private EmbedBuilder GenerateMessageEmbed()
        {
            var embed = new EmbedBuilder()
                .WithTitle($"{Creator.Username} called a ready check")
                .AddField("Ready Users:", ReadyUsersToString(), true)
                .WithFooter("Use the reactions to ready up, email will send a pm when people are ready.\n" + 
                            "Creator can hit the No Vacancy button to conclude the check.");

            AddGameSpecificEmbedOptions(embed);

            return embed;
        }

        private string ReadyUsersToString()
        {
            return  _readyUsers.Count > 0 ? 
                _readyUsers.Aggregate("", 
                    (current, user) => current + Environment.NewLine + user.Username) : "--None--";
        }

        private void AddGameSpecificEmbedOptions(EmbedBuilder embed)
        {
            switch (_game)
            {
                case Game.ProjectWinter:
                    embed.WithTitle($"{embed.Title} for Project Winter!")
                         .WithColor(Color.Blue)
                         .WithImageUrl("https://steamcdn-a.akamaihd.net/steam/apps/774861/header.jpg");
                    break;

                case Game.Dota:
                    embed.WithTitle($"{embed.Title} for Dota!")
                        .WithColor(Color.DarkRed)
                        .WithImageUrl("https://steamcdn-a.akamaihd.net/steam/apps/570/header.jpg");
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