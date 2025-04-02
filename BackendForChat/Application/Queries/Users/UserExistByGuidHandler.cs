using Azure.Core;
using BackendForChat.Models.DatabaseContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackendForChat.Application.Queries.Users
{
    public class UserExistByGuidHandler : IRequestHandler<UserExistByGuidQuery, bool>
    {
        private readonly ApplicationDbContext _context;

        public UserExistByGuidHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> Handle(UserExistByGuidQuery request, CancellationToken cancellationToken)
        {
            return await _context.Users.AnyAsync(u => u.Id == request.Id);
        }
    }
}
