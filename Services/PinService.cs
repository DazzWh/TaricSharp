using System.Linq;
using System.Reflection.Metadata.Ecma335;
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
            if (channel.GetMessageAsync(message.Id).Result is IUserMessage usrMsg)
            {
                var pinCount = usrMsg.Reactions
                    .Where(pair => EmoteIsAPin(pair.Key))
                    .Select(e => e.Value.ReactionCount)
                    .Sum();

                if (pinCount >= Constants.AmountOfPinsNeededToPin)
                    await usrMsg.PinAsync();
            }
        }

        private static bool EmoteIsAPin(IEmote emote)
        {
            return emote.Name.Equals("📌") || emote.Name.Equals("📍");
        }
    }
}