using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Discord;
using Discord.WebSocket;

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
            LogGamesLoaded();
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
            
            return null; // Return blank
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

            return games;     
        }

        private void LogGamesLoaded()
        {
            Log?.Invoke(new LogMessage(LogSeverity.Info, nameof(GameService), $"Loaded {_games.Count} GameInfos"));
        }
    }
}