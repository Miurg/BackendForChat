using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BackendForChat.Application.Common;
using BackendForChat.Application.Services;
using BackendForChat.Models.DTO.Requests;
using BackendForChat.Models.DTO.Response;

namespace BackendForChat.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly AuthService _authService;
        public AuthController(UserService userService, AuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RequestRegisterDto requestRegister)
        {
            if (await _userService.UserExistsByNameAsync(requestRegister.Username))
            {
                return BadRequest("User already exists");
            }

            ServiceResult<ResponseRegisterDto> responseRegister = await _authService.RegisterAsync(requestRegister);

            return CreatedAtAction(nameof(GetUserById), new { id = responseRegister.Data.Id }, responseRegister.Data);
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] RequestLoginDto requestLogin)
        {
            Debug.WriteLine("Acces try");
            ServiceResult<ResponseLoginDto> responseLogin = await _authService.AuthenticateUserAsync(requestLogin);

            if (!responseLogin.Success)
            {
                return Unauthorized(new { message = responseLogin.ErrorMessage });
            } 

            return Ok(responseLogin.Data);
        }

    }

}
