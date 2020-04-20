using System.ComponentModel.DataAnnotations;

namespace activitiesapp.Models.UserDTO
{
    public class UserLoginDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string Salt { get; set; }
    }
}