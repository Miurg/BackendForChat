using BackendForChat.Hubs;
using BackendForChat.Models;
using BackendForChat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackendForChat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly EncryptionService _encryptionService;
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly MessageService _messageService;
        public MessagesController(EncryptionService encryptionService, IHubContext<MessageHub> hubContext, MessageService messageService)
        {
            _encryptionService = encryptionService;
            _hubContext = hubContext;
            _messageService = messageService;
        }

        //[Authorize]
        //[HttpGet]
        //public async Task<IActionResult> GetMessages()
        //{
        //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        //    if (userIdClaim == null)
        //    {
        //        return Unauthorized("User not authenticated");
        //    }
        //    int userId = int.Parse(userIdClaim.Value);

        //    var encryptedMessages = await _context.Messages.Where(x => x.UserId == userId).ToListAsync();

        //    if (encryptedMessages == null || encryptedMessages.Count == 0)
        //    {
        //        return NotFound("No messages");
        //    }

        //    var decryptedMessages = encryptedMessages.Select(message => new
        //    {
        //        message.Id,
        //        Content = _encryptionService.Decrypt(message.Content),
        //        message.UserId,
        //        message.CreatedAt
        //    });

        //    return Ok(decryptedMessages);
        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMessageById(int id)
        {
            var message = await _messageService.GetMessageById(id);
            if (message == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                message.Id,
                Content = _encryptionService.Decrypt(message.Content),
                message.UserId,
                message.CreatedAt
            });
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetMessagesPaged(int page = 1, int pageSize = 50)
        {
            if (pageSize > 100) pageSize = 100; 
            if (page < 1) page = 1; 

            var messages = await _messageService.GetMessagesPaged(page, pageSize);

            var decryptedMessages = messages.Select(message => new
            {
                message.Id,
                Content = _encryptionService.Decrypt(message.Content),
                message.UserId,
                message.CreatedAt
            });

            return Ok(decryptedMessages);
        }

        [HttpPost]
        public async Task<IActionResult> PostMessage([FromBody] NewMessageModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Content))
            {
                return BadRequest("Invalid message data.");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User not authenticated");
            }
            int userId = int.Parse(userIdClaim.Value);

            var message = await _messageService.SaveMessageAsync(model, userId);

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", new
            {
                message.Id,
                Content = model.Content, // Отправляем клиенту уже расшифрованное сообщение
                message.UserId,
                message.CreatedAt
            });

            return CreatedAtAction(nameof(GetMessageById), new { id = message.Id }, new
            {
                message.Id,
                Content = model.Content, // Отправляем клиенту уже расшифрованное сообщение
                message.UserId,
                message.CreatedAt
            });
        }
    }
}
