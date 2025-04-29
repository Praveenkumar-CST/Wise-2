using Microsoft.AspNetCore.Mvc;
using WiseHRServer.Models;
using WiseHRServer.Services;

namespace WiseHRServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeDetailsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeeDetailsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("EmployeeDetailsRegistry")]
        public async Task<IActionResult> EmployeeDetailsRegistry([FromBody] EmployeeDetails employeeDetails)
        {
            if (employeeDetails == null)
            {
                return BadRequest("EmployeeDetails is null");
            }

            Console.WriteLine($"Received Employee: {employeeDetails.FirstName}, Email: {employeeDetails.CurrentEmail}");

            bool registered = await _context.RegisterEmployeeDetailsAsync(employeeDetails);
            return Ok(registered);
        }

        [HttpGet("GetEmployeeDetails/{employeeId}")]
        public async Task<IActionResult> GetEmployeeDetails(string employeeId)
        {
            var EmployeeDetails = await _context.GetEmployeeDetailsAsync(employeeId);
            return Ok(EmployeeDetails);
        }

        [HttpDelete("DeleteEmployeeDetails/{employeeId}")]
        public async Task<IActionResult> DeleteEmployeeDetails(string employeeId)
        {
            bool deleted = await _context.DeleteEmployeeDetailsAsync(employeeId);
            return Ok(deleted);
        }

        [HttpPost("UpdateEmployeeDetails")]
        public async Task<IActionResult> UpdateEmployeeDetails([FromBody] EmployeeDetails employeeDetails)
        {
            bool updated = await _context.UpdateEmployeeDetailsAsync(employeeDetails);
            return Ok(updated);
        }

        [HttpGet("GetAllEmployees")]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _context.GetAllEmployeesAsync();  // Call the service method to fetch all employees
            return Ok(employees);  // Return the list of employees as a response
        }

        [HttpGet("GetEmployeeDetailsByEmail/{email}")]
        public async Task<IActionResult> GetEmployeeDetailsByEmail(string email)
        {
            var employee = await _context.GetEmployeeDetailsByEmailAsync(email);

            if (employee == null)
            {
                return NotFound("Employee not found.");
            }

            return Ok(employee);
        }

    }
}