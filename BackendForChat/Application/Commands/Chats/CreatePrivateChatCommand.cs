using BackendForChat.Application.Common;
using BackendForChat.Application.DTO.Responses;
using MediatR;

namespace BackendForChat.Application.Commands.Chats
{
    public record CreatePrivateChatCommand(Guid FirstUserId, Guid SecondUserId) : IRequest<ServiceResult<ResponseChatCreateDto>>;
}
