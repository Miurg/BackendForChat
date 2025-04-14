using BackendForChat.Application.Common;
using BackendForChat.Application.DTO.Responses;
using BackendForChat.Application.Queries.Users;
using BackendForChat.Models.DatabaseContext;
using BackendForChat.Models.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackendForChat.Application.Commands.Chats
{
    public class CreateGroupChatHandler : IRequestHandler<CreateGroupChatCommand, ServiceResult<ResponseChatCreateDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMediator _mediator;

        public CreateGroupChatHandler(ApplicationDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<ServiceResult<ResponseChatCreateDto>> Handle(CreateGroupChatCommand request, CancellationToken cancellationToken)
        {
            foreach (Guid guid in request.userIds)
            {
                if (!await _mediator.Send(new UserExistByGuidQuery(guid)))
                {
                    return ServiceResult<ResponseChatCreateDto>.Fail("User with that id doesn't exist");
                }
            }
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
