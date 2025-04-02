using MediatR;

namespace BackendForChat.Application.Queries.Users
{
    public record UserExistByGuidQuery(Guid Id) : IRequest<bool>;
}
