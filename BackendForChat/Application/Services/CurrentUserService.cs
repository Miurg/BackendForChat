using BackendForChat.Application.DTO;
using BackendForChat.Application.Interfaces;

namespace BackendForChat.Application.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        private CurrentUserInfo? Info =>
            _httpContextAccessor.HttpContext?.Items["CurrentUser"] as CurrentUserInfo;

        public Guid UserId => Info?.UserId ?? throw new UnauthorizedAccessException("User not authenticated");
    }
}
