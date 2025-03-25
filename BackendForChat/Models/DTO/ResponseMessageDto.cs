using BackendForChat.Services;

namespace BackendForChat.Models.DTO
{
    public class ResponseMessageDto
    {
        public int Id { get; set; }
        public Guid ChatId { get; set; }
        public string Content { get; set; }
        public Guid SenderId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}