using BackendForChat.Models;
using BackendForChat.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackendForChat.Services;

namespace BackendForChat.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserService _userService;
        private readonly AuthService _authService;
        PasswordHasher<UserModel> passwordHasher = new PasswordHasher<UserModel>();
        public AuthController(ApplicationDbContext context, IConfiguration configuration, UserService userService, AuthService authService)
        {
            _context = context;
            _configuration = configuration;
            _userService = userService;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (await _userService.UserExistsAsync(model.Username))
            {
                return BadRequest("User already exists");
            }

            var user = new UserModel
            {
                Username = model.Username,
                PasswordHash = passwordHasher.HashPassword(null, model.Password)
            };

            int newUserId = await _userService.CreateUserAsync(user);

            return CreatedAtAction(nameof(GetUserById), new { id = newUserId }, new { user.Id, user.Username });
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            Debug.WriteLine("Acces try");
            var (success, errorMessage, token) = await _authService.AuthenticateUserAsync(model.Username, model.Password);

            if (!success)
            {
                return Unauthorized(new { message = errorMessage });
            } 
            return Ok(new { token });
        }

    }

}
