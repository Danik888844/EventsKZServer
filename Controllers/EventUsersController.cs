using EventsServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventsServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventUsersController : ControllerBase
    {
        private DatabaseContext _context;
        public EventUsersController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("get_user_events/{id}")]
        public int[] GetUsers_Event(Guid id)
        {
            return _context.EventUsers.Where(x => x.UserId == id).Select(x => x.EventId).ToArray();
        }

        [HttpGet]
        [Route("get_event_users/{id}")]
        public Guid[] GetEvents_User(int id)
        {
            return _context.EventUsers.Where(x => x.EventId == id).Select(x => x.UserId).ToArray();
        }

        [Route("{id}/{userId}")]
        [HttpDelete]
        public IActionResult RemoveRecord(int id, Guid userId)
        {
            var result =  _context.EventUsers.FirstOrDefault(eu => eu.EventId == id && eu.UserId == userId);
            if (result != null)
            {
                _context.EventUsers.Remove(result);
                _context.SaveChanges();
                return Ok("Deleted!");
            }

            return NotFound("Event or User not found!");
        }

        [Route("{id}/{userId}")]
        [HttpPost]
        public IActionResult AddUser(int id, Guid userId)
        {
            try
            {
                var _event = _context.Events.Find(id);
                var _user = _context.Users.Find(userId);
                var eventUser = new EventUser();
                if (_event == null || _user == null)
                {
                    return NotFound("Event or User not found!");
                }
                eventUser.Event = _event;
                eventUser.User = _user;

                _event.EventUsers.Add(eventUser);
                _user.EventUsers.Add(eventUser);

                _context.SaveChanges();
                return Ok("Added!");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating event " + ex);
            }
        }
    }
}
