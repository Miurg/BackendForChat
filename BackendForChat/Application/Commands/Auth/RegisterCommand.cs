using BackendForChat.Application.Common;
using BackendForChat.Application.DTO.Requests;
using BackendForChat.Application.DTO.Responses;
using MediatR;

namespace BackendForChat.Application.Commands.Auth
{
    public record RegisterCommand(RequestRegisterDto requestRegister) : IRequest<ServiceResult<ResponseRegisterDto>>;
}
