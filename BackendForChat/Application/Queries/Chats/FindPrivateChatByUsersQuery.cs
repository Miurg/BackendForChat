using BackendForChat.Application.Common;
using BackendForChat.Application.DTO.Requests;
using BackendForChat.Application.DTO.Responses;
using MediatR;

namespace BackendForChat.Application.Queries.Chats
{
    public record FindPrivateChatByUsersQuery(Guid firtsUserId, Guid secondUserId) : IRequest<ServiceResult<ResponseChatCreateDto>>;
}
