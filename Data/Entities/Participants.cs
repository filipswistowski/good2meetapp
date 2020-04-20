namespace activitiesapp.Models
{
    public class Participants
    {
        public int EventId { get; set; }
        public int UserId { get; set; }

        public User User { get; set; }
        public Event Event { get; set; }
    }
}