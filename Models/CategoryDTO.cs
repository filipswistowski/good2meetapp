using System.ComponentModel.DataAnnotations;

namespace activitiesapp.Models
{
    public class CategoryDTO
    {
        [Required]
        public string CategoryName { get; set; }
    }
}