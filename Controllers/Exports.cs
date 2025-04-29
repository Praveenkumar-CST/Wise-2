using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static AttendanceBackend.Controllers.ExportController;

namespace AttendanceBackend.Controllers
{
    // Model for database schema
    public class AttendanceRecord
    {
        [Key]
        public int Id { get; set; }
        public required string UserId { get; set; }
        public required string Name { get; set; }
        public string Timestamp { get; set; } = ""; // Changed to string
        public int Status { get; set; }

        // Parse string Timestamp to DateTime
        public DateTime ParsedTimestamp => DateTime.TryParse(Timestamp, out var result) ? result : DateTime.MinValue;
    }

    // DbContext for database access
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<AttendanceRecord> AttendanceLogs { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExportController(AppDbContext context)
        {
            _context = context;
        }

        // Model for Excel export
        public class AttendanceDay
        {
            public DateTime Date { get; set; }
            public DateTime? ClockIn { get; set; }
            public DateTime? ClockOut { get; set; }
            public double TotalHours { get; set; }
            public bool Status { get; set; }
            public bool IsWeeklyOff { get; set; }
            public required string Name { get; set; }
        }

        [HttpGet("excel")]
        public async Task<IActionResult> DownloadExcel()
        {
            try
            {
                // Fetch records
                var records = await _context.AttendanceLogs.ToListAsync();

                // Log records for debugging
                Console.WriteLine($"Retrieved {records.Count} records from AttendanceLogs.");
                foreach (var record in records)
                {
                    Console.WriteLine($"Id: {record.Id}, UserId: {record.UserId}, Name: {record.Name}, Status: {record.Status}, Timestamp: {record.Timestamp}, ParsedTimestamp: {record.ParsedTimestamp}");
                }

                if (!records.Any())
                {
                    return Ok("No attendance records available to export.");
                }

                var attendanceData = ProcessAttendanceLogs(records);

                return GenerateExcel(attendanceData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DownloadExcel: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return StatusCode(500, $"An error occurred: {ex.Message}, StackTrace: {ex.StackTrace}");
            }
        }

        [HttpPost("export")]
        public IActionResult Export([FromBody] List<AttendanceDto> logs)
        {
            try
            {
                // Log received data
                Console.WriteLine($"Received {logs.Count} logs for export.");
                foreach (var log in logs)
                {
                    Console.WriteLine($"UserId: {log.UserId}, Name: {log.Name}, Status: {log.Status}, Timestamp: {log.Timestamp}");
                }

                if (!logs.Any())
                {
                    return BadRequest("No data provided for export.");
                }

                // Convert DTOs to AttendanceRecords
                var records = LogsToRecords(logs);
                var attendanceData = ProcessAttendanceLogs(records);

                return GenerateExcel(attendanceData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Export: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return StatusCode(500, $"An error occurred: {ex.Message}, StackTrace: {ex.StackTrace}");
            }
        }

        private List<AttendanceRecord> LogsToRecords(List<AttendanceDto> logs)
        {
            var records = new List<AttendanceRecord>();
            for (int i = 0; i < logs.Count; i++)
            {
                records.Add(new AttendanceRecord
                {
                    Id = i + 1, // Temporary ID for processing
                    UserId = logs[i].UserId,
                    Name = logs[i].Name,
                    Timestamp = logs[i].Timestamp.ToString("yyyy-MM-dd HH:mm:ss"), // Convert DateTime to string
                    Status = logs[i].Status
                });
            }
            return records;
        }

        private List<AttendanceDay> ProcessAttendanceLogs(List<AttendanceRecord> records)
        {
            var attendanceDays = new List<AttendanceDay>();

            var groupedRecords = records
                .GroupBy(r => new { r.UserId, Date = r.ParsedTimestamp.Date })
                .OrderBy(g => g.Key.Date)
                .ThenBy(g => g.Key.UserId);

            foreach (var group in groupedRecords)
            {
                var orderedRecords = group.OrderBy(r => r.ParsedTimestamp).ToList();
                var firstRecord = orderedRecords.First();

                var attendanceDay = new AttendanceDay
                {
                    Date = group.Key.Date,
                    Name = firstRecord.Name ?? "Unknown",
                    Status = orderedRecords.Any(r => r.Status == 1),
                    IsWeeklyOff = IsWeeklyOff(group.Key.Date),
                    ClockIn = orderedRecords.First().ParsedTimestamp,
                    ClockOut = orderedRecords.Last().ParsedTimestamp
                };

                if (attendanceDay.ClockIn.HasValue && attendanceDay.ClockOut.HasValue)
                {
                    attendanceDay.TotalHours = (attendanceDay.ClockOut.Value - attendanceDay.ClockIn.Value).TotalHours;
                }

                attendanceDays.Add(attendanceDay);
            }

            Console.WriteLine($"Processed {attendanceDays.Count} attendance days.");
            return attendanceDays;
        }

        private IActionResult GenerateExcel(List<AttendanceDay> attendanceData)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Attendance");

            // Set headers
            worksheet.Cell(1, 1).Value = "Date";
            worksheet.Cell(1, 2).Value = "Name";
            worksheet.Cell(1, 3).Value = "Clock In";
            worksheet.Cell(1, 4).Value = "Clock Out";
            worksheet.Cell(1, 5).Value = "Total Hours";
            worksheet.Cell(1, 6).Value = "Status"; // Fixed syntax error
            worksheet.Cell(1, 7).Value = "Weekly Off";

            // Populate data rows
            for (int i = 0; i < attendanceData.Count; i++)
            {
                var row = i + 2;
                worksheet.Cell(row, 1).Value = attendanceData[i].Date.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 2).Value = attendanceData[i].Name;
                worksheet.Cell(row, 3).Value = attendanceData[i].ClockIn?.ToString("hh:mm tt") ?? "-";
                worksheet.Cell(row, 4).Value = attendanceData[i].ClockOut?.ToString("hh:mm tt") ?? "-";
                worksheet.Cell(row, 5).Value = attendanceData[i].TotalHours.ToString("F2");
                worksheet.Cell(row, 6).Value = attendanceData[i].Status ? "Present" : "Absent";
                worksheet.Cell(row, 7).Value = attendanceData[i].IsWeeklyOff ? "Yes" : "No";
            }

            // Save to memory stream
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            var fileName = $"Attendance_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        private bool IsWeeklyOff(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Sunday;
        }
    }
}