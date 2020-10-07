using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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
        private static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);

        private readonly Dictionary<ulong, List<LateUser>> _data = new Dictionary<ulong, List<LateUser>>();

        public LateUserDataService(LoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        public async Task Initialise()
        {
            if (File.Exists(Constants.LateUsersFilePath))
            {
                await LoadData();
            }
            else
            {
                await SaveData();
            }
        }

        private async Task LoadData()
        {
            await SemaphoreSlim.WaitAsync();
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
                await _loggingService.Log(
                    new LogMessage(
                        LogSeverity.Error,
                        nameof(LateUserDataService),
                        e.Message));
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        private async Task SaveData()
        {
            await SemaphoreSlim.WaitAsync();
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
                await _loggingService.Log(
                    new LogMessage(
                        LogSeverity.Error,
                        nameof(LateUserDataService),
                        e.Message));
            }
            finally
            {
                SemaphoreSlim.Release();
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
            var user = usersInGuild.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                user = new LateUser(userId);
                _data[guildId].Add(user);
            }
            
            user.Count++;
            
            await SaveData();
        }
        
        //Todo: Increment multiple users with only one SaveData call
    }
}