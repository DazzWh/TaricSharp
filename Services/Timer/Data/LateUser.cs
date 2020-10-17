using System.Xml.Serialization;

namespace TaricSharp.Services.Timer.Data
{
    [XmlType("LateUser")]
    public class LateUser
    {
        [XmlElement("id")] public ulong Id;    // UserID
        [XmlElement("late")] public uint LateCount; // Amount of times late
        [XmlElement("ontime")] public uint OnTimeCount; // Amount of times on time

        public LateUser() { }
        
        public LateUser(ulong id)
        {
            Id = id;
            LateCount = 0;
            OnTimeCount = 0;
        }
    }
}