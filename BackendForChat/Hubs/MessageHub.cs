using BackendForChat.Application.Services;
using BackendForChat.Models.DTO.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BackendForChat.Hubs
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly MessageService _messageService;
        public MessageHub(MessageService messageService) 
        {
            _messageService = messageService;
        }
        //public override async Task OnConnectedAsync()
        //{
        //    string user = Context.User.Identity.Name;
        //    //await Clients.All.SendAsync("SystemMessage", $"User {user} - connected to chat");
        //    RequestMessageDto messageModel = new RequestMessageDto() {Content = $"User {user} - connected to chat" };
        //    var message = await _messageService.SaveMessageAsync(messageModel, 9);
        //    await Clients.All.SendAsync("ReceiveMessage", new
        //    {
        //        message.Id,
        //        Content = messageModel.Content, // Отправляем клиенту уже расшифрованное сообщение
        //        message.SenderId,
        //        message.CreatedAt
        //    });
        //}
        public async Task SendMessage(RequestMessageDto model)
        {
            var userIdClaim = Context.User.FindFirst(ClaimTypes.NameIdentifier);
            Guid userId = Guid.Parse(userIdClaim.Value);
            var message = await _messageService.SaveMessageAsync(model, userId);
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
