using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace TaricSharp.Services
{
    /// <summary>
    /// Pins any message that has a certain number of :pushpin: reactions 
    /// </summary>
    public class PinService
    {
        private const int AmountOfPinsNeededToPin = 2;
        private readonly DiscordSocketClient _client;

        public PinService(DiscordSocketClient client)
        {
            _client = client;
        }

        public void Initialize()
        {
            _client.ReactionAdded += HandleReactionAsync;
        }

        private static async Task HandleReactionAsync(
            Cacheable<IUserMessage, ulong> message,
            ISocketMessageChannel channel,
            SocketReaction reaction)
        {
            if (reaction.Emote.Name.Equals("📌") &&
                channel.GetMessageAsync(message.Id).Result is IUserMessage usrMsg &&
                usrMsg.Reactions.Count(r => r.Key.Name.Equals("📌")) >= AmountOfPinsNeededToPin)
            {
                await usrMsg.PinAsync();
            }
        }
    }
}