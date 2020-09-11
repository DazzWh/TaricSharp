using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;

namespace TaricSharp.Services.Games
{
    public class GameService
    {
        public event Func<LogMessage, Task> Log;

        private List<GameInfo> _games;
        private readonly LoggingService _loggingService;

        public GameService(LoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        public void Initialize()
        {
            _games = GetGameInfo();
            LogGamesLoaded();
        }

        public GameInfo GetGameFromRole(string roleName)
        {
            throw new NotImplementedException();
        }

        private List<GameInfo> GetGameInfo()
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

        private void LogEmptyGameList()
        {
            Log?.Invoke(new LogMessage(LogSeverity.Warning, nameof(GameService), $"No GameInfo found"));
        }
    }
}