using System;

namespace BackendForChat.Models
{
    public class ChatUserModel
    {
        public Guid ChatId { get; set; }
        public ChatModel Chat { get; set; }
        public Guid UserId { get; set; }
        public UserModel User { get; set; }
    }
}
