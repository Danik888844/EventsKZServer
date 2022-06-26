using Microsoft.EntityFrameworkCore;

namespace EventsServer.Models
{
    [Owned]
    public class Coordinates
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
