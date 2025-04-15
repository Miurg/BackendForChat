using BackendForChat.Application.Common;
using BackendForChat.Application.DTO.Responses;
using BackendForChat.Application.Interfaces;
using BackendForChat.Application.Services;
using BackendForChat.Models.DatabaseContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackendForChat.Application.Queries.Messages
{
    public class GetMessagesPagedHandler : IRequestHandler<GetMessagesPagedQuery, ServiceResult<List<ResponseMessageDto>>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IEncryptionService _encryptionService;
        private readonly ICurrentUserService _currentUserService;
        public GetMessagesPagedHandler(ApplicationDbContext context, IEncryptionService encryptionService, ICurrentUserService currentUserService)
        {
            _context = context;
            _encryptionService = encryptionService;
            _currentUserService = currentUserService;
        }
        public async Task<ServiceResult<List<ResponseMessageDto>>> Handle(GetMessagesPagedQuery request, CancellationToken cancellationToken)
        {
            bool userInChat = await _context.ChatUsers
               .AnyAsync(uc => uc.ChatId == request.chatId && uc.UserId == _currentUserService.UserId, cancellationToken);
            if (!userInChat)
            {
                return ServiceResult<List<ResponseMessageDto>>.Fail("User doesn't belong to chat");
            }

            var messages = await _context.Messages
               .Where(message => message.ChatId == request.chatId)
               .OrderByDescending(message => message.CreatedAt) // Last message first
               .Skip((request.page - 1) * request.pageSize)
               .Take(request.pageSize)
               .ToListAsync(cancellationToken);
            if (messages.Count == 0)
                return ServiceResult<List<ResponseMessageDto>>.Fail("Messages with that paged parameters doesn't exist");

            List<ResponseMessageDto> responseMessages = messages.Select(m => new ResponseMessageDto
            {
                Id = m.Id,
                ChatId = m.ChatId,
                Content = _encryptionService.Decrypt(m.Content),
                SenderId = m.SenderId,
                CreatedAt = m.CreatedAt
            }).ToList();

            return ServiceResult<List<ResponseMessageDto>>.Ok(responseMessages);
        }
    }
}
