using BackendForChat.Application.Common;
using BackendForChat.Application.DTO.Responses;
using MediatR;

namespace BackendForChat.Application.Queries.Chats
{
    public record GetUserChatsQuery() : IRequest<ServiceResult<List<ResponseChatCreateDto>>>;
}
