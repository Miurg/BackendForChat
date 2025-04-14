using BackendForChat.Application.Common;
using BackendForChat.Application.DTO.Requests;
using BackendForChat.Application.DTO.Responses;
using BackendForChat.Application.Interfaces;
using BackendForChat.Application.Queries.Users;
using BackendForChat.Application.Services;
using BackendForChat.Models.DatabaseContext;
using BackendForChat.Models.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackendForChat.Application.Commands.Messages
{
    public class SaveMessageHandler: IRequestHandler<SaveMessageCommand, ServiceResult<ResponseMessageDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IEncryptionService _encryptionService;
        private readonly IMediator _mediator;

        public SaveMessageHandler(ApplicationDbContext context, IEncryptionService encryptionService, IMediator mediator)
        {
            _context = context;
            _encryptionService = encryptionService;
            _mediator = mediator;
        }
        public async Task<ServiceResult<ResponseMessageDto>> Handle(SaveMessageCommand request, CancellationToken cancellationToken)
        {
            if (!await _mediator.Send(new UserExistByGuidQuery(request.userId)))
            {
                return ServiceResult<ResponseMessageDto>.Fail("User with that id doesn't exist");
            }

            bool userInChat = await _context.ChatUsers
               .AnyAsync(uc => uc.ChatId == request.newMessage.ChatId && uc.UserId == request.userId, cancellationToken);
            if (!userInChat)
            {
                return ServiceResult<ResponseMessageDto>.Fail("User doesn't belong to chat");
            }

            MessageModel message = new MessageModel
            {
                ChatId = request.newMessage.ChatId,
                Content = _encryptionService.Encrypt(request.newMessage.Content),
                SenderId = request.userId,
                CreatedAt = DateTime.UtcNow
            };
            _context.Messages.Add(message);
            await _context.SaveChangesAsync(cancellationToken);
            ResponseMessageDto responseMessage = new ResponseMessageDto
            {
                Id = message.Id,
                ChatId = message.ChatId,
                Content = _encryptionService.Decrypt(message.Content),
                SenderId = message.SenderId,
                CreatedAt = message.CreatedAt
            };
            return ServiceResult<ResponseMessageDto>.Ok(responseMessage);
        }
    }
}

