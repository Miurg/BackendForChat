using Azure.Core;
using BackendForChat.Application.Common;
using BackendForChat.Application.DTO.Responses;
using BackendForChat.Application.Queries.Messages;
using BackendForChat.Application.Services;
using BackendForChat.Models.DatabaseContext;
using BackendForChat.Models.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackendForChat.Application.Queries.Users
{
    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, ServiceResult<ResponseUserDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetUserByIdHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ServiceResult<ResponseUserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            UserModel user = await _context.Users.FirstOrDefaultAsync(m => m.Id == request.id, cancellationToken);
            if (user == null)
            {
                return ServiceResult<ResponseUserDto>.Fail("User with that id doesn't exist");
            }
            ResponseUserDto responseUser = new ResponseUserDto()
            {
                Id = user.Id,
                Username = user.Username
            };
            return ServiceResult<ResponseUserDto>.Ok(responseUser);
        }
    }
}
