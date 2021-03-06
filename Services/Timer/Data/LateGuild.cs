﻿using System.Collections.Generic;
using System.Xml.Serialization;

namespace TaricSharp.Services.Timer.Data
{
    [XmlType("LateGuild")]
    public class LateGuild
    {
        [XmlElement("id")] public ulong Id; // GuildID
        
        [XmlArray("LateUsers"), XmlArrayItem(typeof(LateUser), ElementName = "User")] 
        public List<LateUser> LateUsers;
    }
}