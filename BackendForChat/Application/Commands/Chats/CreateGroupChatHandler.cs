using BackendForChat.Application.Common;
using BackendForChat.Application.DTO.Responses;
using BackendForChat.Models.DatabaseContext;
using BackendForChat.Models.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackendForChat.Application.Commands.Chats
{
    public class CreateGroupChatHandler : IRequestHandler<CreateGroupChatCommand, ServiceResult<ResponseChatCreateDto>>
    {
        private readonly ApplicationDbContext _context;

        public CreateGroupChatHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<ResponseChatCreateDto>> Handle(CreateGroupChatCommand request, CancellationToken cancellationToken)
        {
            var chatType = await _context.ChatTypes
                .FirstOrDefaultAsync(ct => ct.Type == "Group", cancellationToken);

            if (chatType == null)
            {
                return ServiceResult<ResponseChatCreateDto>.Fail("Chat type not found");
            }

            var chat = new ChatModel
            {
                ChatTypeId = chatType.Id,
                ChatType = chatType
            };

            var chatUsers = request.userIds.Select(userId => new ChatUserModel
            {
                UserId = userId,
                Chat = chat
            }).ToList();

            chat.ChatUsers = chatUsers;

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync(cancellationToken);
            return ServiceResult<ResponseChatCreateDto>.Ok(new ResponseChatCreateDto { ChatId = chat.Id });
        }
    }
}
