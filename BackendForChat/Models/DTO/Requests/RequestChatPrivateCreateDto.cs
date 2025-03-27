using System.ComponentModel.DataAnnotations;

namespace BackendForChat.Models.DTO.Requests
{
    public class RequestChatPrivateCreateDto
    {
        [Required]
        public Guid userId { get; set; }
    }
}
