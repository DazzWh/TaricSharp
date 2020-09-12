using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using Discord;

namespace TaricSharp.Services.Games
{
    [XmlType("gameinfo")]
    public class GameInfo
    {
        [XmlElement("name")]
        public string GameName;
        
        [XmlElement("role")]
        public string RoleName;
        
        [XmlElement("color")]
        public string Color; //Todo: handle this
        
        [XmlElement("image")]
        public string ImageUrl;
    }
}