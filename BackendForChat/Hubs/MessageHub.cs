﻿using BackendForChat.Models;
using BackendForChat.Services;
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
        public override async Task OnConnectedAsync()
        {
            string user = Context.User.Identity.Name;
            await Clients.All.SendAsync("SystemMessage", $"User {user} - connected to chat");
        }
        public async Task SendMessage(NewMessageModel model)
        {
            var userIdClaim = Context.User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = int.Parse(userIdClaim.Value);
            var message = await _messageService.SaveMessageAsync(model, userId);
            await Clients.All.SendAsync("ReceiveMessage", new
            {
                message.Id,
                Content = model.Content, // Отправляем клиенту уже расшифрованное сообщение
                message.UserId,
                message.CreatedAt
            });
        }
    }
}
