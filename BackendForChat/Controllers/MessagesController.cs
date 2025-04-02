using BackendForChat.Application.Commands.Messages;
using BackendForChat.Application.DTO.Requests;
using BackendForChat.Application.Queries.Messages;
using BackendForChat.Application.Services;
using BackendForChat.Hubs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace BackendForChat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHubContext<MessageHub> _hubContext;
        public MessagesController(IHubContext<MessageHub> hubContext, IMediator mediator)
        {
            _mediator = mediator;
            _hubContext = hubContext;
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
            var message = await _mediator.Send(new GetMessageByIdQuery(id, userId));
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

            var messages = await _mediator.Send(new GetMessagesPagedQuery(page, pageSize, userId, chatId));

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

            var message = await _mediator.Send(new SaveMessageCommand(model, userId));
            if (!message.Success)
            {
                return BadRequest(new { error = message.ErrorMessage });
            }

            await _hubContext.Clients.Group(message.Data.ChatId.ToString()).SendAsync("ReceiveMessage", message.Data);

            return CreatedAtAction(nameof(GetMessageById), new { id = message.Data.Id }, message.Data);
        }
    }
}
