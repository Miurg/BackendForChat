using BackendForChat.Application.Common;
using BackendForChat.Application.DTO.Requests;
using BackendForChat.Application.DTO.Responses;
using MediatR;

namespace BackendForChat.Application.Commands.Messages
{
    public record SaveMessageCommand(RequestMessageDto newMessage, Guid userId) : IRequest<ServiceResult<ResponseMessageDto>>;
}

