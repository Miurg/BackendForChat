using BackendForChat.Models.DatabaseContext;
using BackendForChat.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendForChat.Application.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserModel> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }
        public async Task<bool> UserExistsByNameAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
        public async Task<bool> UserExistByGuidAsync(Guid Id)
        {
            return await _context.Users.AnyAsync(u => u.Id == Id);
        }
    }
}
