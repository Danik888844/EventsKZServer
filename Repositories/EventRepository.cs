using EventsServer.Models;
using Microsoft.EntityFrameworkCore;

namespace EventsServer.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly DatabaseContext _context;

        public EventRepository(DatabaseContext appDbContext)
        {
            this._context = appDbContext;
        }

        public async Task<IEnumerable<Event>> GetEvents()
        {
            return await _context.Events.ToListAsync();
        }

        public async Task<Event> GetEvent(int eventId)
        {
            return await _context.Events
                .FirstOrDefaultAsync(e => e.Id == eventId);
        }

        public async Task<Event> AddEvent(Event _event)
        {
            var result = await _context.Events.AddAsync(_event);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Event> UpdateEvent(Event new_event)
        {
            var _event = await _context.Events
                .FirstOrDefaultAsync(e => e.Id == new_event.Id);

            if (_event != null)
            {
                _event.Name = new_event.Name;
                _event.Description = new_event.Description;
                _event.Photos = new_event.Photos;
                _event.Price = new_event.Price;
                _event.Numbers = new_event.Numbers;
                _event.Coordinates = new_event.Coordinates;
                _event.EventDate = new_event.EventDate;
                _event.isApproved = new_event.isApproved;

                await _context.SaveChangesAsync();

                return _event;
            }

            return null;
        }

        public async Task<Event> DeleteEvent(int eventId)
        {
            var result = await _context.Events
                .FirstOrDefaultAsync(e => e.Id == eventId);
            if (result != null)
            {
                _context.Events.Remove(result);
                await _context.SaveChangesAsync();
                return result;
            }

            return null;
        }

        public async Task<int> DeleteEventWhereOwner(Guid ownerId)
        {
            var result = await _context.Events
                .Where(e => e.Owner == ownerId).ToArrayAsync();
            if (result != null)
            {
                for(int i = 0; i < result.Length; i++)
                {
                    _context.Events.Remove(result[i]);
                }
                
                await _context.SaveChangesAsync();
                return result.Length;
            }

            return result.Length;
        }
    }
}
