using BackendForChat.Application.Common;
using BackendForChat.Application.DTO.Responses;
using MediatR;

namespace BackendForChat.Application.Queries.Messages
{
    public record GetMessagesPagedQuery(int page, int pageSize, Guid userId, Guid chatId) : IRequest<ServiceResult<List<ResponseMessageDto>>>;
}
