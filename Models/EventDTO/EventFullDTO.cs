using System;
using System.ComponentModel.DataAnnotations;

namespace activitiesapp.Models
{
    public class EventFullDTO
    {
        public int EventId { get; set; }

        [Required]
        public string EventName { get; set; }

        [Required]
        public string EventDescription { get; set; }

        public int UserId { get; set; }

        [Required]
        public string EventPlace { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        public int CategoryId { get; set; }
        public int CurrentEventParticipants { get; set; }

        [Required]
        public int MaxEventParticipants { get; set; }
    }
}