using System;
using System.Xml.Serialization;

namespace TaricSharp.Services.PersistantData
{
    [XmlType("LateUser")]
    public class LateUser
    {
        [XmlElement("id")] public ulong Id;    // UserID
        [XmlElement("count")] public uint Count; // Amount of times late

        public LateUser() { }
        
        public LateUser(ulong id)
        {
            Id = id;
            Count = 0;
        }
    }
}