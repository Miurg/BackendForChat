using BackendForChat.Application.Common;
using BackendForChat.Application.DTO.Responses;
using BackendForChat.Models.DatabaseContext;
using BackendForChat.Models.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackendForChat.Application.Queries.Users
{
    public class UserExistsByNameHandler : IRequestHandler<UserExistsByNameQuery, bool>
    {
        private readonly ApplicationDbContext _context;

        public UserExistsByNameHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> Handle(UserExistsByNameQuery request, CancellationToken cancellationToken)
        {
            return await _context.Users.AnyAsync(u => u.Username == request.username);
        }
    }
}
