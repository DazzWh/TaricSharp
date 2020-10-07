using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Discord;
using TaricSharp.Extensions;
using TaricSharp.Services.Games;

namespace TaricSharp.Services.PersistantData
{
    /// <summary>
    /// A service for storing data related to users being late, used by timer service
    /// </summary>
    public class LateUserDataService
    {
        private readonly LoggingService _loggingService;
        // Todo: Async key solution, check camera branch for example

        private Dictionary<ulong, List<LateUser>> _data = new Dictionary<ulong, List<LateUser>>();

        public LateUserDataService(LoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        public void Initialise()
        {
            if (File.Exists(Constants.LateUsersFilePath))
            {
                LoadData();
            }
            else
            {
                _data = new Dictionary<ulong, List<LateUser>>()
                {
                    { 0, new List<LateUser>() }
                };
                SaveData();
            }
        }

        private void LoadData()
        {
            _data.Clear();

            try
            {
                var serializer = new XmlSerializer(typeof(List<LateGuild>));
                var reader = new StreamReader(Constants.LateUsersFilePath);

                var guilds = (List<LateGuild>) serializer.Deserialize(reader);

                foreach (var guild in guilds)
                {
                    var users = guild.LateUsers;
                    _data.Add(guild.Id, users);
                }

                reader.Close();
            }
            catch (Exception e)
            {
                _loggingService.Log(
                    new LogMessage(
                        LogSeverity.Error, 
                        nameof(LateUserDataService), 
                        e.Message));
            }
        }

        private void SaveData()
        {
            try
            {
                var lateGuilds = new List<LateGuild>(_data.Count);
                lateGuilds.AddRange(
                    _data.Keys.Select(key => new LateGuild {Id = key, LateUsers = _data[key]})
                    );
                
                var writer = new StreamWriter(Constants.LateUsersFilePath);
                var serializer = new XmlSerializer(typeof(List<LateGuild>));
                serializer.Serialize(writer, lateGuilds);
                writer.Close();
            }
            catch (Exception e)
            {
                _loggingService.Log(
                    new LogMessage(
                        LogSeverity.Error, 
                        nameof(LateUserDataService), 
                        e.Message));
            }
        }

        /// <summary>
        /// Used by TimerEndService to track how often users have been late
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="guildId"></param>
        public async Task IncrementLateUser(ulong userId, ulong guildId)
        {
            var usersInGuild = _data.GetOrCreate(guildId);
            var user = usersInGuild.FirstOrDefault(u => u.Id == userId) ?? new LateUser(userId);
            
            user.Count++;
            
            SaveData();
        }
    }
}