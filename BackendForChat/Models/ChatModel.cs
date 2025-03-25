using System.ComponentModel.DataAnnotations;

namespace BackendForChat.Models
{
    public class ChatModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int ChatTypeId { get; set; }
        public ChatTypeModel ChatType { get; set; }
        public ICollection<ChatUserModel> ChatUsers { get; set; } = [];
        public ICollection<MessageModel> Messages { get; set; } = [];
    }
}
