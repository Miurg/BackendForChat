using BackendForChat.Application.Common;
using BackendForChat.Application.DTO.Responses;
using BackendForChat.Models.DatabaseContext;
using BackendForChat.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BackendForChat.Application.Commands.Auth
{
    public class RegisterHandler : IRequestHandler<RegisterCommand, ServiceResult<ResponseRegisterDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<UserModel> _passwordHasher;

        public RegisterHandler(ApplicationDbContext context, IPasswordHasher<UserModel> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }
        public async Task<ServiceResult<ResponseRegisterDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var user = new UserModel
            {
                Username = request.requestRegister.Username,
                PasswordHash = _passwordHasher.HashPassword(null, request.requestRegister.Password)
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);
            ResponseRegisterDto responseRegister = new ResponseRegisterDto { Id = user.Id, Username = user.Username };
            return ServiceResult<ResponseRegisterDto>.Ok(responseRegister);
        }
    }
}
