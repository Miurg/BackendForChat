using BackendForChat.Models.Entities;

namespace BackendForChat.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateJwtToken(UserModel user);
    }
}
