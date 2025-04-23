using BackendForChat.Application.Common;
using BackendForChat.Application.DTO.Responses;
using BackendForChat.Application.Interfaces;
using BackendForChat.Models.DatabaseContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackendForChat.Application.Queries.Chats
{
    public class GetUserChatsHandler : IRequestHandler<GetUserChatsQuery, ServiceResult<List<ResponseChatCreateDto>>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        public GetUserChatsHandler(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }
        public async Task<ServiceResult<List<ResponseChatCreateDto>>> Handle(GetUserChatsQuery request, CancellationToken cancellationToken)
        {
            var chats = await _context.ChatUsers
                       .Where(uc => uc.UserId == _currentUserService.UserId)
                       .Select(uc => uc.Chat)
                       .ToListAsync(cancellationToken);
            if (!chats.Any())
                return ServiceResult<List<ResponseChatCreateDto>>.Fail("User not have chats");
            List<ResponseChatCreateDto> chatResponse = new List<ResponseChatCreateDto>();
            foreach (var chatEntity in chats)
            {
                chatResponse.Add(new ResponseChatCreateDto
                {
                    ChatId = chatEntity.Id
                });
            }
            return ServiceResult<List<ResponseChatCreateDto>>.Ok(chatResponse);
        }
    }
}
