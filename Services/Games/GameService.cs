using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Discord;
using Discord.WebSocket;
using TaricSharp.Extensions;

namespace TaricSharp.Services.Games
{
    public class GameService
    {
        public event Func<LogMessage, Task> Log;
        private List<GameInfo> _games;

        public void Initialize()
        {
            _games = GetGameInfo();
            Log?.Invoke(new LogMessage(LogSeverity.Info, nameof(GameService), $"Loaded {_games.Count} GameInfos"));
        }

        public GameInfo GetGameFromMentions(IEnumerable<SocketRole> mentions)
        {
            foreach (var mention in mentions)
            {
                var game = _games.FirstOrDefault(g => g.RoleName.Equals(mention.Name));
                if (game != null)
                {
                    return game;
                }
            }

            return null;
        }

        private List<GameInfo> GetGameInfo()
        {
            var games = new List<GameInfo>();

            try
            {
                var serializer = new XmlSerializer(typeof(List<GameInfo>), new XmlRootAttribute("games"));
                var stringReader = new StreamReader(Constants.GamesListFilePath);
                games = (List<GameInfo>) serializer.Deserialize(stringReader);
            }
            catch (Exception e)
            {
                Log?.Invoke(new LogMessage(LogSeverity.Error, nameof(GameService), e.Message));
            }

            return ValidateGameInfo(games);
        }

        private List<GameInfo> ValidateGameInfo(IReadOnlyCollection<GameInfo> games)
        {
            var validGames = new List<GameInfo>(games);

            foreach (var game in games)
            {
                if (ValidGameInfo(game)) continue;
                
                validGames.Remove(game);
                Log?.Invoke(new LogMessage(
                    LogSeverity.Error,
                    nameof(GameService),
                    $"{game.GameName} GameInfo invalid, not loaded."));
            }

            return validGames;
        }

        private static bool ValidGameInfo(GameInfo game)
        {
            return ValidGameName(game) &&
                   ValidGameRole(game) &&
                   ValidGameColor(game) &&
                   ValidGameImageUrl(game);
        }

        private static bool ValidGameName(GameInfo game)
        {
            return game.GameName.Length > 0 && game.GameName.Length < Constants.MaxRoleNameLength;
        }

        private static bool ValidGameRole(GameInfo game)
        {
            return game.RoleName.Length > 0 && game.RoleName.Length < Constants.MaxRoleNameLength;
        }
        
        private static bool ValidGameColor(GameInfo game)
        {
            return game.ColorValue.IsValidHexString();
        }
        
        private static bool ValidGameImageUrl(GameInfo game)
        {
            return game.ImageUrl.IsNullOrUri();
        }
    }
}