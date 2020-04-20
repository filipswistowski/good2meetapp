using System.ComponentModel.DataAnnotations;

namespace activitiesapp.Models
{
    public class AuthenticateDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}