using Microsoft.AspNetCore.Mvc;
using WiseHRServer.Models;
using WiseHRServer.Services;

namespace WiseHRServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExperienceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExperienceController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("ExperienceRegistry")]
        public async Task<IActionResult> ExperienceRegistry([FromBody] Experience experience)
        {
            bool created = await _context.RegisterExperienceAsync(experience);
            return Ok(created);
        }

        [HttpGet("GetExperienceInfo/{employeeId}")]
        public async Task<IActionResult> GetExperience(string employeeId)
        {
            var EmployeeDetails = await _context.GetExperienceAsync(employeeId);
            return Ok(EmployeeDetails);
        }

        [HttpDelete("DeleteExperience/{employeeId}")]
        public async Task<IActionResult> DeleteExperience(string employeeId)
        {
            bool deleted = await _context.DeleteExperienceAsync(employeeId);
            return Ok(deleted);
        }

        [HttpPost("UpdateExperience")]
        public async Task<IActionResult> UpdateExperience([FromBody] Experience experience)
        {
            bool updated = await _context.UpdateExperienceAsync(experience);
            return Ok(updated);
        }
        [HttpGet("GetAllExperience")]
        public async Task<IActionResult> GetAllExperienceInfo()
        {
            var EmployeeDetails = await _context.GetAllExperienceAsync();  // Call the service method to fetch all employees
            return Ok(EmployeeDetails);  // Return the list of employees as a response
        }
    }
}