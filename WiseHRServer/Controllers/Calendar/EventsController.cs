using Microsoft.AspNetCore.Mvc;
using CalendarApi.Data;
using CalendarApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Threading.Tasks;

namespace CalendarApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Tags("Calendar Events")]
    public class EventsController : ControllerBase
    {
        private readonly CalendarDbContext _context;

        public EventsController(CalendarDbContext context)
        {
            _context = context;
        }

        // GET: api/events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            var events = await _context.Events.ToListAsync();
            return Ok(events);
        }

        // GET: api/events/2025-03-31
        [HttpGet("{date}")]
        public async Task<ActionResult<Event>> GetEvent(string date)
        {
            var evt = await _context.Events.FirstOrDefaultAsync(e => e.Date == date);
            if (evt == null) return NotFound();
            return Ok(evt);
        }

        // POST: api/events
        [HttpPost]
        public async Task<ActionResult<Event>> CreateEvent([FromBody] Event evt)
        {
            if (evt == null || string.IsNullOrEmpty(evt.Date) || string.IsNullOrEmpty(evt.HolidayType))
            {
                return BadRequest("Date and HolidayType are required.");
            }

            if (!IsValidHolidayType(evt.HolidayType))
            {
                return BadRequest("Invalid HolidayType. Allowed values are 'Permanent' or 'Optional'.");
            }

            // Automatically set the Day field
            if (DateTime.TryParseExact(evt.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                evt.Day = parsedDate.DayOfWeek.ToString();
            }
            else
            {
                return BadRequest("Invalid date format. Use 'yyyy-MM-dd'.");
            }

            _context.Events.Add(evt);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEvent), new { date = evt.Date }, evt);
        }

        // PUT: api/events/2025-03-31
        [HttpPut("{date}")]
        public async Task<IActionResult> UpdateEvent(string date, [FromBody] Event updatedEvent)
        {
            if (updatedEvent == null || string.IsNullOrEmpty(updatedEvent.Date) || string.IsNullOrEmpty(updatedEvent.HolidayType))
            {
                return BadRequest("Date and HolidayType are required.");
            }

            if (!IsValidHolidayType(updatedEvent.HolidayType))
            {
                return BadRequest("Invalid HolidayType. Allowed values are 'Permanent' or 'Optional'.");
            }

            var existingEvent = await _context.Events.FirstOrDefaultAsync(e => e.Date == date);
            if (existingEvent == null) return NotFound();

            // Update the event properties
            existingEvent.Description = updatedEvent.Description;
            existingEvent.HolidayType = updatedEvent.HolidayType;

            // Automatically set the Day field
            if (DateTime.TryParseExact(updatedEvent.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                existingEvent.Day = parsedDate.DayOfWeek.ToString();
            }
            else
            {
                return BadRequest("Invalid date format. Use 'yyyy-MM-dd'.");
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/events/2025-03-31
        [HttpDelete("{date}")]
        public async Task<IActionResult> DeleteEvent(string date)
        {
            var evt = await _context.Events.FirstOrDefaultAsync(e => e.Date == date);
            if (evt == null) return NotFound();

            _context.Events.Remove(evt);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool IsValidHolidayType(string holidayType)
        {
            return holidayType == "Permanent" || holidayType == "Optional";
        }
    }
}
