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
        private readonly DiscordSocketClient _client;

        public PinService(DiscordSocketClient client)
        {
            _client = client;
        }

        public void Initialize()
        {
            _client.ReactionAdded += HandlePinReactionAsync;
        }

        private static async Task HandlePinReactionAsync(
            Cacheable<IUserMessage, ulong> message,
            ISocketMessageChannel channel,
            SocketReaction reaction)
        {
            if (reaction.Emote.Name.Equals("📌") &&
                channel.GetMessageAsync(message.Id).Result is IUserMessage usrMsg &&
                usrMsg.Reactions.Count(
                    r => r.Key.Name.Equals("📌")) >= Constants.AmountOfPinsNeededToPin)
            {
                await usrMsg.PinAsync();
            }
        }
    }
}