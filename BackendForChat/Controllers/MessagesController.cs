using BackendForChat.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackendForChat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public MessagesController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User not authenticated");
            }
            int userId = int.Parse(userIdClaim.Value);

            var messages = await _context.Messages.Where(x => x.UserId == userId).ToListAsync();

            if (messages == null || messages.Count == 0)
            {
                return NotFound("No messages");
            }
            return Ok(messages);
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

            var message = new MessageModel
            {
                Content = model.Content,
                UserId = userId,  
                CreatedAt = DateTime.UtcNow  
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMessages), new { id = message.Id }, message);
        }
    }
}
