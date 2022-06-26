namespace EventsServer.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public byte[] Photos { get; set; } = new byte[0];
        public int Price { get; set; } = 0;
        public string[] Numbers{ get; set; } = new string[0];
        public Coordinates Coordinates { get; set; } = new Coordinates();
        public DateTime EventDate { get; set; } = DateTime.Now;
        public bool isApproved { get; set; } = false;
        public Guid Owner { get; set; }
        public List<EventUser> EventUsers { get; set; } = new List<EventUser>();
    }
}
