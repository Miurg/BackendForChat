using BackendForChat.Application.Common;
using BackendForChat.Models.DatabaseContext;
using BackendForChat.Models.DTO.Requests;
using BackendForChat.Models.DTO.Response;
using BackendForChat.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BackendForChat.Application.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<UserModel> _passwordHasher;
        private readonly JwtService _jwtService;
        public AuthService(ApplicationDbContext context, IPasswordHasher<UserModel> passwordHasher, JwtService jwtService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<ServiceResult<ResponseLoginDto>> AuthenticateUserAsync(RequestLoginDto requestLogin)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == requestLogin.Username);
            if (user == null)
            {
                return ServiceResult<ResponseLoginDto>.Fail("Invalid credentials");
            }

            if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, requestLogin.Password) != PasswordVerificationResult.Success)
            {
                return ServiceResult<ResponseLoginDto>.Fail("Invalid credentials");
            }

            var token = _jwtService.GenerateJwtToken(user);
            ResponseLoginDto responseLogin = new ResponseLoginDto { Token = token };
            return ServiceResult<ResponseLoginDto>.Ok(responseLogin);
        }
        public async Task<ServiceResult<ResponseRegisterDto>> RegisterAsync(RequestRegisterDto requestRegister)
        {
            var user = new UserModel
            {
                Username = requestRegister.Username,
                PasswordHash = _passwordHasher.HashPassword(null, requestRegister.Password)
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            ResponseRegisterDto responseRegister = new ResponseRegisterDto { Id = user.Id, Username = user.Username };
            return ServiceResult<ResponseRegisterDto>.Ok(responseRegister);
        }
    }
}
