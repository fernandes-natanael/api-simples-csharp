using AwesomeDevEvents.API.Entities;
using AwesomeDevEvents.API.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AwesomeDevEvents.API.Controllers
{

    [Route("api/dev-events")]
    [ApiController]
    public class DevEventsController : ControllerBase
    {
        private readonly DevEventsDBContext _context;
        public DevEventsController(DevEventsDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var devEvents = _context.Events.Where(d => !d.IsDeleted).ToList();
            return Ok(devEvents);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var devEvents = _context.Events
                .Include(de => de.Speakers)
                .SingleOrDefault(d => d.Id == id);
            if (devEvents == null)
            {
                return NotFound();
            }
            return Ok(devEvents);
        }

        [HttpPost]
        public IActionResult Post(DevEvent devEvent)
        {
            _context.Events.Add(devEvent);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = devEvent.Id }, devEvent);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, DevEvent devEvent)
        {
            var devEventSearched = _context.Events.SingleOrDefault(d => d.Id == id);
            if (devEventSearched == null)
            {
                return NotFound();
            }

            devEventSearched.Update(devEvent.Title, devEvent.Description, devEvent.StartDate, devEvent.EndDate);
            _context.Events.Update(devEventSearched);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var devEvent = _context.Events.SingleOrDefault(e => e.Id == id);
            if (devEvent == null)
                return NotFound();

            devEvent.Delete();
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPost("{id}/speakers")]
        public IActionResult PostSpeaker(Guid id, DevEventSpeaker devEventSpeaker)
        {
            var devEvent = _context.Events.Any(e => e.Id == id);
            if (!devEvent)
                return NotFound();

            _context.EventsSpeakers.Add(devEventSpeaker);
            _context.SaveChanges();

            return NoContent();
        }

    }

}