using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace TaricSharp.Services.Games
{
    public class GameService
    {
        public event Func<LogMessage, Task> Log;
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
                var game = _games.First(g => g.RoleName.Equals(mention.Name));
                if (game != null)
                {
                    return game;
                }
            }
            
            return GameInfo.None;
        }

        private static List<GameInfo> GetGameInfo()
        {
            var games = typeof(GameInfo).GetFields()
                .Where(f => f.FieldType == typeof(GameInfo))
                .Select(field => (GameInfo) field.GetValue(null))
                .Where(game => game != null)
                .ToList();

            return games;
        }

        private void LogGamesLoaded()
        {
            Log?.Invoke(new LogMessage(LogSeverity.Info, nameof(GameService), $"Loaded {_games.Count} GameInfos"));
        }
    }
}