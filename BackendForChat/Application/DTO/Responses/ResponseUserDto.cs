using BackendForChat.Models.Entities;

namespace BackendForChat.Application.DTO.Responses
{
    public class ResponseUserDto
    {
        public Guid Id { get; set; } 
        public string Username { get; set; }
    }
}
