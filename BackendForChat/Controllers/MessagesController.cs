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

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetMessageById(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }
            Guid userId = Guid.Parse(userIdClaim.Value);
            var message = await _messageService.GetMessageById(id, userId);
            if (!message.Success)
            {
                return NotFound(new { error = message.ErrorMessage });
            }

            return Ok(message.Data);
        }

        [HttpGet("paged/{chatId:guid}")]
        public async Task<IActionResult> GetMessagesPaged(Guid chatId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (pageSize > 100 || pageSize < 1) pageSize = 100; 
            if (page < 1) page = 1;

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }
            Guid userId = Guid.Parse(userIdClaim.Value);

            var messages = await _messageService.GetMessagesPaged(page, pageSize, userId, chatId);

            if (!messages.Success)
            {
                return NotFound(new { error = messages.ErrorMessage });
            }

            return Ok(messages.Data);
        }

        [HttpPost]
        public async Task<IActionResult> PostMessage([FromBody] RequestMessageDto model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Content) || model.ChatId == Guid.Empty)
            {
                
                return BadRequest(new { message = "Invalid message data." });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }
            Guid userId = Guid.Parse(userIdClaim.Value);

            var message = await _messageService.SaveMessageAsync(model, userId);
            if (!message.Success)
            {
                return BadRequest(new { error = message.ErrorMessage });
            }

            await _hubContext.Clients.Group(message.Data.ChatId.ToString()).SendAsync("ReceiveMessage", message.Data);

            return CreatedAtAction(nameof(GetMessageById), new { id = message.Data.Id }, message.Data);
        }
    }
}
