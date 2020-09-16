using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace TaricSharp.Services
{
    /// <summary>
    /// A service for storing data that needs to be persisted between restarts
    /// </summary>
    public class PersistentDataService
    {
        private const string Filename = "data.xml";
        private static string FilenamePath => Path.Combine(Directory.GetCurrentDirectory(), Filename);

        private readonly XmlDocument _data = new XmlDocument();

        // Todo: Async key solution, check camera branch for example

        public PersistentDataService()
        {

        }

        public void Initialise()
        {
            if (File.Exists(FilenamePath))
            {
                _data.Load(FilenamePath);
            }
            else
            {
                _data.CreateXmlDeclaration("1.0", "UTF-8", null);
                _data.Save(FilenamePath);
            }
        }

        private void SaveData()
        {
            _data.Save(FilenamePath);
        }

        /// <summary>
        /// Used by TimerEndService to track how often users have been late
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="serverId"></param>
        public async Task IncrementLateUser(ulong userId, ulong guildId)
        {
            const string lateUserNodeName = "LateUsers";

            // Todo: Create or load the relevant data here and increment by one
        }
    }
}