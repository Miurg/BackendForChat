namespace BackendForChat.Models
{
    public class MessageModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int UserId { get; set; }
        public UserModel User { get; set; }
        public DateTime CreatedAt { get; set; } 
    }
}
