using BackendForChat.Application.Commands.Chats;
using BackendForChat.Application.DTO.Requests;
using BackendForChat.Application.Queries.Chats;
using BackendForChat.Application.Queries.Users;
using BackendForChat.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackendForChat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatsController : Controller
    {
        private readonly IMediator _mediator;
        public ChatsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("get-or-create")]
        public async Task<IActionResult> GetOrCreatePrivateChatAsync([FromBody] RequestChatPrivateCreateDto chatRequest)
        {
            if (!(await _mediator.Send(new UserExistByGuidQuery(chatRequest.userId))))
            {
                return BadRequest(new { error = "User already exists" });
            }
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            Guid userId = Guid.Parse(userIdClaim.Value);

            var chat = await _mediator.Send(new FindPrivateChatByUsersQuery(userId,chatRequest.userId));

            if (chat.Success)
            {
                return Ok(chat.Data); 
            }

            chat = await _mediator.Send(new CreatePrivateChatCommand(userId, chatRequest.userId));
            if (!chat.Success)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            return CreatedAtAction(nameof(GetChatById),new { id = chat.Data.ChatId },chat.Data);
            
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetChatById(Guid id)
        {
            var chat = await _mediator.Send(new GetChatByGuidQuery(id));

            if (!chat.Success)
            {
                return NotFound(chat.ErrorMessage);
            }

            return Ok(chat.Data);
        }
    }
}
