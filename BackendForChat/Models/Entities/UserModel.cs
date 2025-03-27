namespace BackendForChat.Models.Entities
{
    public class UserModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public ICollection<ChatUserModel> ChatUsers { get; set; } = [];
        public ICollection<MessageModel> SentMessages { get; set; } = [];

    }
}
