using System.ComponentModel.DataAnnotations;

namespace BackendForChat.Application.DTO.Requests
{
    public class RequestLoginDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
