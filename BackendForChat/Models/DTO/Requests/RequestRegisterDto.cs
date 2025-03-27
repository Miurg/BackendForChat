using System.ComponentModel.DataAnnotations;

namespace BackendForChat.Models.DTO.Requests
{
    public class RequestRegisterDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
