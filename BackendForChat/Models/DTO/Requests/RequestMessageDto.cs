using System.ComponentModel.DataAnnotations;

namespace BackendForChat.Models.DTO.Requests
{
    public class RequestMessageDto
    {
        [Required]
        public Guid ChatId { get; set; }
        [Required]
        public string Content { get; set; }
    }
}
