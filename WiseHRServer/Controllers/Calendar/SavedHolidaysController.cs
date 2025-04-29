using Microsoft.AspNetCore.Mvc;
using CalendarApi.Data;
using CalendarApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CalendarApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SavedHolidaysController : ControllerBase
    {
        private readonly CalendarDbContext _context;

        public SavedHolidaysController(CalendarDbContext context)
        {
            _context = context;
        }

        // POST: api/SavedHolidays
        [HttpPost]
        public async Task<IActionResult> SaveHolidays([FromBody] SavedHoliday[] holidays)
        {
            if (holidays == null || !holidays.Any())
            {
                return BadRequest("No holidays provided to save.");
            }

            try
            {
                foreach (var holiday in holidays)
                {
                    bool exists = await _context.SavedHolidays.AnyAsync(h => h.Date == holiday.Date && h.UserEmail == holiday.UserEmail);
                    if (!exists)
                    {
                        _context.SavedHolidays.Add(holiday);
                    }
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = $"Successfully saved {holidays.Length} holidays" });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error saving holidays: {ex.Message}");
                return StatusCode(500, new { message = $"Error saving holidays: {ex.Message}" });
            }
        }

        // GET: api/SavedHolidays/{userEmail}
        [HttpGet("{userEmail}")]
        public async Task<ActionResult<IEnumerable<SavedHoliday>>> GetSavedHolidays(string userEmail)
        {
            var savedHolidays = await _context.SavedHolidays
                .Where(h => h.UserEmail == userEmail)
                .ToListAsync();

            var sortedHolidays = savedHolidays
                .Where(sh => DateTime.TryParse(sh.Date, out _)) // Ensure valid dates
                .OrderBy(sh => DateTime.Parse(sh.Date)) // Perform sorting in-memory
                .ToList();

            return Ok(sortedHolidays);
        }

        // DELETE: api/SavedHolidays/{userEmail}/{date}
        [HttpDelete("{userEmail}/{date}")]
        public async Task<IActionResult> DeleteHoliday(string userEmail, string date)
        {
            var holiday = await _context.SavedHolidays
                .FirstOrDefaultAsync(h => h.UserEmail == userEmail && h.Date == date);
            if (holiday == null)
            {
                return NotFound(new { message = "Holiday not found" });
            }

            _context.SavedHolidays.Remove(holiday);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Holiday deleted successfully" });
        }

        // PUT: api/SavedHolidays/{userEmail}/{date}
        [HttpPut("{userEmail}/{date}")]
        public async Task<IActionResult> UpdateHoliday(string userEmail, string date, [FromBody] SavedHoliday updatedHoliday)
        {
            if (userEmail != updatedHoliday.UserEmail || date != updatedHoliday.Date)
            {
                return BadRequest("Mismatched holiday user email or date");
            }

            var holiday = await _context.SavedHolidays
                .FirstOrDefaultAsync(h => h.UserEmail == userEmail && h.Date == date);
            if (holiday == null)
            {
                return NotFound(new { message = "Holiday not found" });
            }

            holiday.EventName = updatedHoliday.EventName;

            _context.SavedHolidays.Update(holiday);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Holiday updated successfully" });
        }
    }
}