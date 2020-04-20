using System.ComponentModel.DataAnnotations;

namespace activitiesapp.Models
{
    public class UserFullDTO
    {
        public int UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }

        public string Email { get; set; }
        public int UserTypeId { get; set; }
        public string Token { get; set; }
        public string Salt { get; set; }
    }
}