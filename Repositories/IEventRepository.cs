using EventsServer.Models;

namespace EventsServer.Repositories
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetEvents();
        Task<Event> GetEvent(int eventId);
        Task<Event> AddEvent(Event _event);
        Task<Event> UpdateEvent(Event _event);
        Task<Event> DeleteEvent(int eventId);
        Task<int> DeleteEventWhereOwner(Guid ownerId);
    }
}
