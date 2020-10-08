using Discord.WebSocket;

namespace TaricSharp.Extensions
{
    public static class SocketReactionExtensions
    {
        public static bool UserNullOrBot(this SocketReaction reaction)
        {
            return !reaction.User.IsSpecified || reaction.User.Value.IsBot;
        }
    }
}