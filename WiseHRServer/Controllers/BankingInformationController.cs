using Microsoft.AspNetCore.Mvc;
using WiseHRServer.Models;
using WiseHRServer.Services;

namespace WiseHRServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BankingInformationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BankingInformationController(AppDbContext context)
        {
            _context = context;
        }

            [HttpPost("BankingInfoRegistry")]
            public async Task<IActionResult> BankingInfoRegistry([FromBody] BankingInformation bankingInfo)
            {
                bool created = await _context.RegisterBankingProfileAsync(bankingInfo);
                return Ok(created);
            }

            [HttpGet("GetBankingInfo/{employeeId}")]
            public async Task<IActionResult> GetBankingInfo(string employeeId)
            {
                var BankingInfo = await _context.GetBankingInformationAsync(employeeId);
                return Ok(BankingInfo);
            }

            [HttpDelete("DeleteBankingInfo/{employeeId}")]
            public async Task<IActionResult> DeleteBankingInfo(string employeeId)
            {
                bool deleted = await _context.DeleteBankingInformationAsync(employeeId);
                return Ok(deleted);
            }

            [HttpPost("UpdateBankingInfo")]
            public async Task<IActionResult> UpdateBankingInfo([FromBody] BankingInformation bankingInfo)
            {
                bool updated = await _context.UpdateBankingInformationAsync(bankingInfo);
                return Ok(updated);
            }

            [HttpGet("GetAllBankingInfo")]
            public async Task<IActionResult> GetAllbankingInfo()
            {
                var bankingdetails = await _context.GetAllBankinginfoAsync();  // Call the service method to fetch all employees
                return Ok(bankingdetails);  // Return the list of employees as a response
            }

    }
}

        