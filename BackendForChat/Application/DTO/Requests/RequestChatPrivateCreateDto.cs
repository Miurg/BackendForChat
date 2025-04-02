using System.ComponentModel.DataAnnotations;

namespace BackendForChat.Application.DTO.Requests
{
    public class RequestChatPrivateCreateDto
    {
        [Required]
        public Guid userId { get; set; }
    }
}
