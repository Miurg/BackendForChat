using BackendForChat.Hubs;
using BackendForChat.Models;
using BackendForChat.Models.DTO;
using BackendForChat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackendForChat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatsController : Controller
    {
        private readonly ChatService _сhatService;
        public ChatsController(ChatService chatService)
        {
            _сhatService = chatService;
        }
        [HttpPost("get-or-create")]
        public async Task<IActionResult> GetOrCreatePrivateChatAsync([FromBody] RequestChatCreateDto chatRequest)
        {
            
            Console.WriteLine(chatRequest.userId);
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User not authenticated");
            }

            Guid userId = Guid.Parse(userIdClaim.Value);
            ResponseChatCreateDto chat = await _сhatService.FindPrivateChatByUsersAsync(userId, chatRequest.userId);

            if (chat != null)
            {
                return Ok(chat); 
            }

            chat = await _сhatService.CreatePrivateChatAsync(userId, chatRequest.userId);
            if (chat != null)
            {
                return CreatedAtAction(
                    nameof(GetChatById),
                    new { id = chat.Id },
                    chat);
            }

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetChatById(Guid id)
        {
            ResponseChatCreateDto chat = await _сhatService.GetChatById(id);

            if (chat == null)
            {
                return NotFound();
            }

            return Ok(chat);
        }
    }
}
