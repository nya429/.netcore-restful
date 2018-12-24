using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTOs
{
    public class UserToRegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "You must specify")]
        public string Password { get; set; }
    }
}