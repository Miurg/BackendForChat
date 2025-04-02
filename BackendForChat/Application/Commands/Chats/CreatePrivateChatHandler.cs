using BackendForChat.Application.Common;
using BackendForChat.Application.DTO.Responses;
using BackendForChat.Models.DatabaseContext;
using BackendForChat.Models.Entities;
using MediatR;
using System;
using Microsoft.EntityFrameworkCore;

namespace BackendForChat.Application.Commands.Chats
{
    public class CreatePrivateChatHandler : IRequestHandler<CreatePrivateChatCommand, ServiceResult<ResponseChatCreateDto>>
    {
        private readonly ApplicationDbContext _context;

        public CreatePrivateChatHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<ResponseChatCreateDto>> Handle(CreatePrivateChatCommand request, CancellationToken cancellationToken)
        {
            var chatType = await _context.ChatTypes
                .FirstOrDefaultAsync(ct => ct.Type == "Private", cancellationToken);

            if (chatType == null)
            {
                return ServiceResult<ResponseChatCreateDto>.Fail("Chat type not found");
            }

            var chat = new ChatModel
            {
                ChatTypeId = chatType.Id,
                ChatType = chatType
            };

            chat.ChatUsers = new List<ChatUserModel>
        {
            new ChatUserModel { UserId = request.FirstUserId, Chat = chat },
            new ChatUserModel { UserId = request.SecondUserId, Chat = chat }
        };

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync(cancellationToken);

            return ServiceResult<ResponseChatCreateDto>.Ok(new ResponseChatCreateDto { ChatId = chat.Id });
        }
    }
}
