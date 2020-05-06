using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Color = Discord.Color;

namespace TaricSharp.Services
{
    public class ReadyCheck
    {
        public readonly RestUserMessage ReadyMsg;
        private readonly int _readyCount;
        private readonly HashSet<IUser> _readyUsers;
        private readonly Game _game;

        public ReadyCheck(
            RestUserMessage readyMsg,
            int readyCount,
            Game game)
        {
            ReadyMsg = readyMsg;
            _readyCount = readyCount;
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

        private async Task UpdateMessage()
        {
            var readyUsers = _readyUsers.Count > 0 ? 
                _readyUsers.Aggregate("", (current, user) => current + Environment.NewLine + user.Username) : "--None--";

            var embed = new EmbedBuilder()
                .WithTitle("Ready Check!")
                .AddField("Ready Users:", readyUsers, true)
                .WithFooter("Use the reactions to ready up, email will send a pm when people are ready");

            AddGameSpecificEmbedOptions(embed);

            await ReadyMsg.ModifyAsync(m =>
            {
                m.Content = "";
                m.Embed = embed.Build();
            });
        }

        private void AddGameSpecificEmbedOptions(EmbedBuilder embed)
        {
            switch (_game)
            {
                case Game.ProjectWinter:
                    embed.WithColor(Color.Blue)
                         .WithImageUrl("https://steamcdn-a.akamaihd.net/steam/apps/774861/header.jpg");
                    break;

                case Game.Dota:
                    embed.WithColor(Color.DarkRed)
                        .WithImageUrl("https://steamcdn-a.akamaihd.net/steam/apps/570/header.jpg");
                    break;

                case Game.None:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task ToggleNotifyOnUser(IUser user)
        {
            throw new NotImplementedException();
        }
    }
}