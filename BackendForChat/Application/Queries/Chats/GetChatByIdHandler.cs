using Azure.Core;
using BackendForChat.Application.Common;
using BackendForChat.Application.DTO.Responses;
using BackendForChat.Models.DatabaseContext;
using BackendForChat.Models.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackendForChat.Application.Queries.Chats
{
    public class GetChatByIdHandler : IRequestHandler<GetChatByIdQuery, ServiceResult<ResponseChatCreateDto>>
    {
        private readonly ApplicationDbContext _context;
        public GetChatByIdHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ServiceResult<ResponseChatCreateDto>> Handle(GetChatByIdQuery request, CancellationToken cancellationToken)
        {
            var chat = await _context.Chats.FindAsync(request.id, cancellationToken);
            if (chat == null)
            {
                return ServiceResult<ResponseChatCreateDto>.Fail("Chat with that id does not exist");
            }
            ResponseChatCreateDto chatResponse = new ResponseChatCreateDto { ChatId = chat.Id };
            return ServiceResult<ResponseChatCreateDto>.Ok(chatResponse);
        }
    }
}
