using BackendForChat.Application.Common;
using BackendForChat.Application.DTO.Responses;
using MediatR;

namespace BackendForChat.Application.Queries.Chats
{
    public record GetChatByIdQuery(Guid id) : IRequest<ServiceResult<ResponseChatCreateDto>>;
}
