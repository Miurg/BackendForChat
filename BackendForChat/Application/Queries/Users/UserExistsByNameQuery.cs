using BackendForChat.Application.Common;
using BackendForChat.Application.DTO.Responses;
using MediatR;

namespace BackendForChat.Application.Queries.Users
{
    public record UserExistsByNameQuery(string username) : IRequest<bool>;
}
