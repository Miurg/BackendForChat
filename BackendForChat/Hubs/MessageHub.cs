using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BackendForChat.Hubs
{
    public class MessageHub : Hub
    {
        public async Task SendMessage(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveMessage", message);
        }
    }
}
