namespace EventsServer.Models
{
    public class EventUser
    {
        public int EventId { get; set; }
        public Event Event { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
