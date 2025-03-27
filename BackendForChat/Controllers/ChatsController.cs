using BackendForChat.Application.Common;
using BackendForChat.Application.Services;
using BackendForChat.Hubs;
using BackendForChat.Models;
using BackendForChat.Models.DTO.Requests;
using BackendForChat.Models.DTO.Response;
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
        private readonly UserService _userService;
        public ChatsController(ChatService chatService, UserService userService)
        {
            _сhatService = chatService;
            _userService = userService;
        }
        [HttpPost("get-or-create")]
        public async Task<IActionResult> GetOrCreatePrivateChatAsync([FromBody] RequestChatPrivateCreateDto chatRequest)
        {
            if (!(await _userService.UserExistByGuidAsync(chatRequest.userId)))
            {
                return BadRequest("User already exists");
            }
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            Guid userId = Guid.Parse(userIdClaim.Value);

            ServiceResult<ResponseChatCreateDto> chat = await _сhatService.FindPrivateChatByUsersAsync(userId, chatRequest.userId);

            if (chat.Success == true)
            {
                return Ok(chat); 
            }

            chat = await _сhatService.CreatePrivateChatAsync(userId, chatRequest.userId);
            if (chat.Success == false)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            return CreatedAtAction(nameof(GetChatById),new { id = chat.Data.ChatId },chat);
            
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
