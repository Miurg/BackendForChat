using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BackendForChat.Hubs
{
    public class MessageHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
