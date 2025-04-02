using BackendForChat.Application.Common;
using BackendForChat.Application.DTO.Responses;
using BackendForChat.Application.Queries.Auth;
using BackendForChat.Application.Services;
using BackendForChat.Models.DatabaseContext;
using BackendForChat.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BackendForChat.Application.Queries.Chats
{
    public class FindPrivateChatByUsersHandler : IRequestHandler<FindPrivateChatByUsersQuery, ServiceResult<ResponseChatCreateDto>>
    {
        private readonly ApplicationDbContext _context;
        public FindPrivateChatByUsersHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<ResponseChatCreateDto>> Handle(FindPrivateChatByUsersQuery request, CancellationToken cancellationToken)
        {
            var chatType = await _context.ChatTypes
                .FirstOrDefaultAsync(ct => ct.Type == "Private", cancellationToken);

            if (chatType == null)
            {
                throw new Exception("Chat type not found");
            }
            ChatModel chat = await _context.Chats
                .Where(chat => chat.ChatType.Id == chatType.Id)
                .Where(chat => chat.ChatUsers.Any(uc => uc.UserId == request.firtsUserId) &&
                      chat.ChatUsers.Any(uc => uc.UserId == request.secondUserId))
                .FirstOrDefaultAsync(cancellationToken);
            if (chat == null)
                return ServiceResult<ResponseChatCreateDto>.Fail("Chat with this user does not exist");
            ResponseChatCreateDto chatResponse = new ResponseChatCreateDto { ChatId = chat.Id };
            return ServiceResult<ResponseChatCreateDto>.Ok(chatResponse);
        }
    }
}
