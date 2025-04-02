using BackendForChat.Application.Common;
using BackendForChat.Application.DTO.Responses;
using MediatR;

namespace BackendForChat.Application.Commands.Chats
{
    public record CreateGroupChatCommand(List<Guid> userIds) : IRequest<ServiceResult<ResponseChatCreateDto>>;
}
