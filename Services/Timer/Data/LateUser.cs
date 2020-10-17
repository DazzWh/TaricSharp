using System.Xml.Serialization;
using JetBrains.Annotations;

namespace TaricSharp.Services.Timer.Data
{
    [XmlType("LateUser")]
    public class LateUser
    {
        [XmlElement("id")] public readonly ulong Id;    // UserID
        [XmlElement("late")] public uint LateCount; // Amount of times late
        [XmlElement("on-time")] public uint OnTimeCount; // Amount of times on time

        [UsedImplicitly]
        public LateUser() { }
        
        public LateUser(ulong id)
        {
            Id = id;
            LateCount = 0;
            OnTimeCount = 0;
        }
    }
}