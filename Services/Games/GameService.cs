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
        private const string GamesListFilePath = "Games.xml";
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
                var stringReader = new StreamReader(GamesListFilePath);
                games = (List<GameInfo>) serializer.Deserialize(stringReader);
            }
            catch (Exception e)
            {
                Log?.Invoke(new LogMessage(LogSeverity.Error, nameof(GameService), e.Message));
            }

            return ValidateGameInfo(games);
        }

        private static List<GameInfo> ValidateGameInfo(IReadOnlyCollection<GameInfo> games)
        {
            var validGames = new List<GameInfo>(games);

            foreach (var game in games)
            {
                if (!ValidGameName(game) ||
                    !ValidGameRole(game) ||
                    !ValidGameImageUrl(game) ||
                    !ValidGameColor(game))
                {
                    validGames.Remove(game);
                }
            }

            return validGames;
        }

        private static bool ValidGameName(GameInfo game)
        {
            // Todo: Make a constants file for things like this.
            return game.GameName.Length > 0 && game.GameName.Length < 100;
        }

        private static bool ValidGameRole(GameInfo game)
        {
            return game.RoleName.Length > 0 && game.RoleName.Length < 100;
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