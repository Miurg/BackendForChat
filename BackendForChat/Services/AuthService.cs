using BackendForChat.Models;
using BackendForChat.Models.DatabaseContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BackendForChat.Services
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

        public async Task<(bool Success, string ErrorMessage, string Token)> AuthenticateUserAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return (false, "Invalid credentials", null);
            }

            if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) != PasswordVerificationResult.Success)
            {
                return (false, "Invalid credentials", null);
            }

            var token = _jwtService.GenerateJwtToken(user);
            return (true, null, token);
        }
    }
}
