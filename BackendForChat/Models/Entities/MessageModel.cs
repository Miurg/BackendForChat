namespace BackendForChat.Models.Entities
{
    public class MessageModel
    {
        public int Id { get; set; }
        public Guid ChatId { get; set; }
        public ChatModel Chat { get; set; } 
        public string Content { get; set; }
        public Guid SenderId { get; set; }
        public UserModel Sender { get; set; }
        public DateTime CreatedAt { get; set; } 
    }
}
