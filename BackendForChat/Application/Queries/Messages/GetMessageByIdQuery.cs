using BackendForChat.Application.Common;
using BackendForChat.Application.DTO.Responses;
using MediatR;

namespace BackendForChat.Application.Queries.Messages
{
    public record GetMessageByIdQuery (int id) : IRequest<ServiceResult<ResponseMessageDto>>;
}
