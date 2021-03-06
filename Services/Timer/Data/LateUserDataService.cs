﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Discord;
using TaricSharp.Extensions;

namespace TaricSharp.Services.Timer.Data
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

        private async Task IncrementUserData(ulong userId, ulong guildId, bool late)
        {
            await SemaphoreSlim.WaitAsync();
            
            var usersInGuild = _data.GetOrCreate(guildId);
            var user = usersInGuild.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                user = new LateUser(userId);
                _data[guildId].Add(user);
            }

            if (late)
            {
                user.LateCount++; 
            }
            else
            {
                user.OnTimeCount++;
            }
            
            SemaphoreSlim.Release();
        }
        
        public async Task IncrementOnTimeUsers(IEnumerable<ulong> userIds, ulong guildId)
        {
            foreach (var id in userIds)
            {
                await IncrementUserData(id, guildId, false);
            }

            await SaveData();
        }
        

        public async Task IncrementLateUsers(IEnumerable<ulong> userIds, ulong guildId)
        {
            foreach (var id in userIds)
            {
                await IncrementUserData(id, guildId, true);
            }

            await SaveData();
        }

        public IEnumerable<LateUser> GetUsersFromGuild(ulong guildId)
        {
            _data.TryGetValue(guildId, out var users);
            return users ?? new List<LateUser>();
        }
    }
}