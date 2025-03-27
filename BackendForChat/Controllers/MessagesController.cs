using BackendForChat.Application.Services;
using BackendForChat.Hubs;
using BackendForChat.Models.DTO.Requests;
using BackendForChat.Models.DTO.Response;
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
                message.SenderId,
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
                message.SenderId,
                message.CreatedAt
            });

            return Ok(decryptedMessages);
        }

        [HttpPost]
        public async Task<IActionResult> PostMessage([FromBody] RequestMessageDto model)
        {
            if (model == null || string.IsNullOrEmpty(model.Content) || string.IsNullOrWhiteSpace(model.ChatId.ToString())
                || Guid.TryParse(model.ChatId.ToString(), out Guid chatId) && chatId == Guid.Empty)
            {
                return BadRequest("Invalid message data.");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User not authenticated");
            }
            Guid userId = Guid.Parse(userIdClaim.Value);

            ResponseMessageDto message = await _messageService.SaveMessageAsync(model, userId);

            await _hubContext.Clients.Group(message.ChatId.ToString()).SendAsync("ReceiveMessage", message);

            return CreatedAtAction(nameof(GetMessageById), new { id = message.Id }, message);
        }
    }
}
