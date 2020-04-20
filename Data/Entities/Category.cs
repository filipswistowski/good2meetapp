using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace activitiesapp.Models
{
    public class Category
    {

        /// <summary>
        /// The Id of the category
        /// </summary>
        [Key]
        public int CategoryId { get; set; }
        
        /// <summary>
        /// Name of the category
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// Event Category reference for foreign key
        /// </summary>
        public virtual ICollection<Event> Events { get; set; }

    }
}
