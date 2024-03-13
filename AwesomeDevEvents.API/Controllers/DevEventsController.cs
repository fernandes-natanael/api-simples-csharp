using AutoMapper;
using AwesomeDevEvents.API.Entities;
using AwesomeDevEvents.API.Models;
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
        private readonly IMapper _mapper;
        public DevEventsController(
            DevEventsDBContext context, 
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var devEvents = _context.Events.Where(d => !d.IsDeleted).ToList();
            var viewModel = _mapper.Map<List<DevEventViewModel>>(devEvents);
            return Ok(viewModel);
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

            var viewModel = _mapper.Map<DevEventViewModel>(devEvents);
            return Ok(viewModel);
        }

        [HttpPost]
        public IActionResult Post(DevEventInputModel input)
        {
            var devEvent = _mapper.Map<DevEvent>(input);
            _context.Events.Add(devEvent);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = devEvent.Id }, devEvent);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, DevEventInputModel devEvent)
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
        public IActionResult PostSpeaker(Guid id, DevEventSpeakerInputModel input)
        {
            var devEventSpeaker = _mapper.Map<DevEventSpeaker>(input);
            var devEvent = _context.Events.Any(e => e.Id == id);
            if (!devEvent)
                return NotFound();

            _context.EventsSpeakers.Add(devEventSpeaker);
            _context.SaveChanges();

            return NoContent();
        }

    }

}