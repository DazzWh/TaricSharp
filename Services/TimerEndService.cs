using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace TaricSharp.Services
{
    public class TimerEndService
    {
        private async Task HandleReactionsAsync(
            Cacheable<IUserMessage, ulong> message,
            ISocketMessageChannel channel,
            SocketReaction reaction)
        {
            //if (!reaction.User.IsSpecified || reaction.User.Value.IsBot)
            //    return;

            //var msg = await channel.GetMessageAsync(message.Id);

            //var endMessage = _endMessages.FirstOrDefault(m => m.Id == msg.Id);

            //if (endMessage != null)
            //{
            //    // Handle end messages reactions
            //}


        }
        
        //private async Task CheckMessages()
        //{
        //    foreach (var msg in
        //        _timerMessages.Where(msg => msg.EndTime < DateTime.Now))
        //    {
        //        var endMsg = (RestUserMessage)await msg.Channel.SendMessageAsync("Ending timer...");
        //        _endMessages.Add(new TimerEndMessage(endMsg));
        //        await msg.FinishMessage();
        //        _timerMessages.Remove(msg);
        //    }

        //    foreach (var msg in
        //        _endMessages.Where(msg => msg.EndTime < DateTime.Now))
        //    {
        //        msg.FinishMessage();
        //        // Increment the database of users who didn't accept in time
        //        _endMessages.Remove(msg);
        //    }
        //}
    }
}