namespace BackendForChat.Models.DTO.Response
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