using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace activitiesapp.Models
{
    public class UserType
    {
        [Key]
        public int UserTypeId { get; set; }
        public string Type { get; set; }

        public ICollection<User> Users { get; set; }
    }
}