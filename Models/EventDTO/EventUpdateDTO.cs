using System;

namespace activitiesapp.Models.EventDTO
{
    public class EventUpdateDTO
    {
        public string EventName { get; set; }
        public string EventDescription { get; set; }
        public string EventPlace { get; set; }
        public DateTime EventDate { get; set; }
        public int CategoryId { get; set; }
        public int CurrentEventParticipants { get; set; }
        public int MaxEventParticipants { get; set; }
    }
}