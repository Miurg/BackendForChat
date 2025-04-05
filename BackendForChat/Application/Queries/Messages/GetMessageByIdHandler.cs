using Azure.Core;
using BackendForChat.Application.Common;
using BackendForChat.Application.DTO.Responses;
using BackendForChat.Application.Interfaces;
using BackendForChat.Application.Queries.Chats;
using BackendForChat.Application.Services;
using BackendForChat.Models.DatabaseContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackendForChat.Application.Queries.Messages
{
    public class GetMessageByIdHandler : IRequestHandler<GetMessageByIdQuery, ServiceResult<ResponseMessageDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IEncryptionService _encryptionService;
        public GetMessageByIdHandler(ApplicationDbContext context, IEncryptionService encryptionService)
        {
            _context = context;
            _encryptionService = encryptionService;
        }
        public async Task<ServiceResult<ResponseMessageDto>> Handle(GetMessageByIdQuery request, CancellationToken cancellationToken)
        {
            var message = await _context.Messages
                .Include(m => m.Chat)
                .ThenInclude(c => c.ChatUsers)
                .FirstOrDefaultAsync(m => m.Id == request.id, cancellationToken);

            if (message == null)
            {
                return ServiceResult<ResponseMessageDto>.Fail("Message with that id doesn't exist");
            }
            if (!message.Chat.ChatUsers.Any(user => user.User.Id == request.userId))
            {
                return ServiceResult<ResponseMessageDto>.Fail("User doesn't belong to chat");
            }

            ResponseMessageDto responseMessage = new ResponseMessageDto
            {
                Id = message.Id,
                ChatId = message.ChatId,
                Content = _encryptionService.Decrypt(message.Content),
                SenderId = message.SenderId,
                CreatedAt = message.CreatedAt,
            };
            return ServiceResult<ResponseMessageDto>.Ok(responseMessage);
        }
    }
}
