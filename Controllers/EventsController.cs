using EventsServer.Models;
using EventsServer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventsServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;

        public EventsController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        [HttpGet]
        public async Task<ActionResult> GetEvents()
        {
            try
            {
                return Ok(await _eventRepository.GetEvents());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            try
            {
                var result = await _eventRepository.GetEvent(id);

                if (result == null)
                {
                    return NotFound();
                }

                return result;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Event>> CreateEvent(Event _event)
        {
            try
            {
                if (_event == null)
                {
                    return BadRequest();
                }

                var createdEvent = await _eventRepository.AddEvent(_event);

                return CreatedAtAction(nameof(GetEvent), new { id = createdEvent.Id },
                    createdEvent);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating event " + ex);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Event>> UpdateEvent(int id, Event new_event)
        {
            try
            {
                if (id != new_event.Id)
                    return BadRequest("Event ID mismatch");

                var eventToUpdate = await _eventRepository.GetEvent(id);

                if (eventToUpdate == null)
                    return NotFound($"Event with Id = {id} not found");

                return await _eventRepository.UpdateEvent(new_event);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Event>> DeleteEvent(int id)
        {
            try
            {
                var eventToDelete = await _eventRepository.GetEvent(id);

                if (eventToDelete == null)
                {
                    return NotFound($"Event with Id = {id} not found");
                }

                return await _eventRepository.DeleteEvent(id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }

        [HttpDelete]
        [Route("delete_where_owner/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> DeleteEventWhereOwner(Guid id)
        {
            try
            {
                return await _eventRepository.DeleteEventWhereOwner(id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }
    }
}
