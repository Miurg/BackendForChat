using BackendForChat.Application.Commands.Messages;
using BackendForChat.Application.DTO.Requests;
using BackendForChat.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BackendForChat.Hubs
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly IMediator _mediator;
        public MessageHub(IMediator mediator) 
        {
            _mediator = mediator;
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
            var message = await _mediator.Send(new SaveMessageCommand(model));
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
