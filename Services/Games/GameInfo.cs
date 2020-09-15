using System.Xml.Serialization;
using Discord;
using TaricSharp.Extensions;

namespace TaricSharp.Services.Games
{
    [XmlType("gameinfo")]
    public class GameInfo
    {
        [XmlElement("name")] public string GameName;
        [XmlElement("role")] public string RoleName;
        [XmlElement("color")] public string ColorValue;
        [XmlElement("image")] public string ImageUrl;
        
        public Color Color => ColorValue.ToColor();
    }
}