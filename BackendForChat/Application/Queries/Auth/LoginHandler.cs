using BackendForChat.Application.Commands.Auth;
using BackendForChat.Application.Common;
using BackendForChat.Application.DTO.Responses;
using BackendForChat.Application.Interfaces;
using BackendForChat.Application.Queries.Chats;
using BackendForChat.Application.Services;
using BackendForChat.Models.DatabaseContext;
using BackendForChat.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BackendForChat.Application.Queries.Auth
{
    public class LoginHandler : IRequestHandler<LoginQuery, ServiceResult<ResponseLoginDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<UserModel> _passwordHasher;
        private readonly IJwtService _jwtService;
        public LoginHandler(ApplicationDbContext context, IPasswordHasher<UserModel> passwordHasher, IJwtService jwtService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<ServiceResult<ResponseLoginDto>> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.requestLogin.Username);
            if (user == null)
            {
                return ServiceResult<ResponseLoginDto>.Fail("Invalid credentials");
            }

            if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.requestLogin.Password) != PasswordVerificationResult.Success)
            {
                return ServiceResult<ResponseLoginDto>.Fail("Invalid credentials");
            }

            var token = _jwtService.GenerateJwtToken(user);
            ResponseLoginDto responseLogin = new ResponseLoginDto { Token = token };
            return ServiceResult<ResponseLoginDto>.Ok(responseLogin);
        }
    }
}
