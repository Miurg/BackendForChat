using System.ComponentModel.DataAnnotations;

namespace BackendForChat.Application.DTO.Requests
{
    public class RequestRegisterDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
