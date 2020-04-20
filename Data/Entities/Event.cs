using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace activitiesapp.Models
{
    public class Event
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int EventId { get; set; }

        public string EventName { get; set; }
        public string EventDescription { get; set; }
        public int UserId { get; set; }
        public string EventPlace { get; set; }
        public DateTime EventDate { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public int CurrentEventParticipants { get; set; }
        public int MaxEventParticipants { get; set; }

        public virtual Category Category { get; set; }
        public virtual User Users { get; set; }
        public virtual ICollection<Participants> Participants { get; set; }
        // public virtual ICollection<Category> Categories { get; set; }


    }
}
