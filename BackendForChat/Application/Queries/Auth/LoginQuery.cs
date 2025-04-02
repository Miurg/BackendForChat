using BackendForChat.Application.Common;
using BackendForChat.Application.DTO.Requests;
using BackendForChat.Application.DTO.Responses;
using MediatR;

namespace BackendForChat.Application.Queries.Auth
{
    public record LoginQuery(RequestLoginDto requestLogin) : IRequest<ServiceResult<ResponseLoginDto>>;
}
