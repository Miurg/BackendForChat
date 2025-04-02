using BackendForChat.Application.Common;
using BackendForChat.Application.DTO.Responses;
using MediatR;

namespace BackendForChat.Application.Queries.Messages
{
    public record GetMessageByIdQuery (int id, Guid userId) : IRequest<ServiceResult<ResponseMessageDto>>;
}
