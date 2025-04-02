using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BackendForChat.Application.Common;
using BackendForChat.Application.Services;
using BackendForChat.Application.DTO.Requests;
using BackendForChat.Application.DTO.Responses;
using MediatR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using BackendForChat.Application.Commands.Auth;
using BackendForChat.Application.Queries.Users;
using BackendForChat.Application.Queries.Auth;

namespace BackendForChat.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RequestRegisterDto requestRegister)
        {
            if (await _mediator.Send(new UserExistsByNameQuery(requestRegister.Username)))
            {
                return BadRequest("User already exists");
            }

            ServiceResult<ResponseRegisterDto> responseRegister = await _mediator.Send(new RegisterCommand(requestRegister));

            return CreatedAtAction(nameof(GetUserById), new { id = responseRegister.Data.Id }, responseRegister.Data);
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _mediator.Send(new GetUserByIdQuery(id));
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
            ServiceResult<ResponseLoginDto> responseLogin = await _mediator.Send(new LoginQuery(requestLogin));

            if (!responseLogin.Success)
            {
                return Unauthorized(new { message = responseLogin.ErrorMessage });
            } 

            return Ok(responseLogin.Data);
        }

    }

}
