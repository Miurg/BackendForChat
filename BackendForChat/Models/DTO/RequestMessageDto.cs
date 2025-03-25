namespace BackendForChat.Models.DTO
{
    public class RequestMessageDto
    {
        public Guid ChatId { get; set; }
        public string Content { get; set; }
    }
}
