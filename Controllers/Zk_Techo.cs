using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace AttendanceBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ZktecoController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ZktecoController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("insert-log")]
        public IActionResult InsertLog([FromBody] AttendanceDto data)
        {
            try
            {
                var connStr = _config.GetConnectionString("DefaultConnection");
                using var conn = new SqlConnection(connStr);
                conn.Open();

                var cmd = new SqlCommand("INSERT INTO AttendanceLogs (UserId, Name, Timestamp, Status) VALUES (@UserId, @Name, @Timestamp, @Status)", conn);
                cmd.Parameters.AddWithValue("@UserId", data.UserId);
                cmd.Parameters.AddWithValue("@Name", data.Name ?? "Unknown");
                cmd.Parameters.AddWithValue("@Timestamp", data.Timestamp);
                cmd.Parameters.AddWithValue("@Status", data.Status);

                cmd.ExecuteNonQuery();
                return Ok("✅ Data inserted");
            }
            catch (Exception ex)
            {
                return BadRequest("❌ Error: " + ex.Message);
            }
        }

        [HttpGet("get-logs/{name}")]
        public IActionResult GetLogsByName(string name)
        {
            try
            {
                var logs = new List<AttendanceDto>();
                var connStr = _config.GetConnectionString("DefaultConnection");
                using var conn = new SqlConnection(connStr);
                conn.Open();

                var cmd = new SqlCommand("SELECT UserId, Name, Timestamp, Status FROM AttendanceLogs WHERE Name = @Name", conn);
                cmd.Parameters.AddWithValue("@Name", name);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    DateTime timestamp;

                    if (reader["Timestamp"] is DateTime dt)
                    {
                        timestamp = dt;
                    }
                    else if (!DateTime.TryParse(reader["Timestamp"].ToString(), out timestamp))
                    {
                        timestamp = DateTime.MinValue;
                    }

                    logs.Add(new AttendanceDto
                    {
                        UserId = reader["UserId"].ToString(),
                        Name = reader["Name"].ToString(),
                        Timestamp = timestamp,
                        Status = Convert.ToInt32(reader["Status"])
                    });
                }

                if (logs.Count == 0)
                {
                    return NotFound(new { Message = $"No attendance records found for name: {name}" });
                }

                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpGet("get-all-logs")]
        public IActionResult GetAllLogs()
        {
            try
            {
                var logs = new List<AttendanceDto>();
                var connStr = _config.GetConnectionString("DefaultConnection");
                using var conn = new SqlConnection(connStr);
                conn.Open();

                var cmd = new SqlCommand("SELECT UserId, Name, Timestamp, Status FROM AttendanceLogs", conn);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    DateTime timestamp;

                    if (reader["Timestamp"] is DateTime dt)
                    {
                        timestamp = dt;
                    }
                    else if (!DateTime.TryParse(reader["Timestamp"].ToString(), out timestamp))
                    {
                        timestamp = DateTime.MinValue;
                    }

                    logs.Add(new AttendanceDto
                    {
                        UserId = reader["UserId"].ToString(),
                        Name = reader["Name"].ToString(),
                        Timestamp = timestamp,
                        Status = Convert.ToInt32(reader["Status"])
                    });
                }

                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpGet("get-all-reasons")]
        public IActionResult GetAllReasons()
        {
            try
            {
                var reasons = new List<LowHoursReasonDto>();
                var connStr = _config.GetConnectionString("DefaultConnection");
                using var conn = new SqlConnection(connStr);
                conn.Open();

                var cmd = new SqlCommand("SELECT UserId, Name, Date, Reason, CreatedAt FROM LowHoursReasons", conn);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    reasons.Add(new LowHoursReasonDto
                    {
                        UserId = reader["UserId"].ToString() ?? "",
                        Name = reader["Name"].ToString() ?? "",
                        Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                        Reason = reader["Reason"].ToString() ?? "",
                    });
                }

                if (reasons.Count == 0)
                {
                    return Ok(new List<LowHoursReasonDto>());
                }

                return Ok(reasons);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"❌ Internal server error: {ex.Message}" });
            }
        }

        [HttpPost("accept-reason")]
        public IActionResult AcceptReason([FromBody] AcceptReasonDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.UserId) || string.IsNullOrWhiteSpace(dto.Name))
                {
                    return BadRequest("❌ Invalid request data: UserId and Name are required.");
                }

                var connStr = _config.GetConnectionString("DefaultConnection");
                using var conn = new SqlConnection(connStr);
                conn.Open();

                // Check if the reason exists and is not already accepted
                var checkCmd = new SqlCommand(
                    "SELECT IsAccepted FROM LowHoursReasons WHERE UserId = @UserId AND Date = @Date",
                    conn);
                checkCmd.Parameters.AddWithValue("@UserId", dto.UserId);
                checkCmd.Parameters.AddWithValue("@Date", dto.Date.Date);

                using var reader = checkCmd.ExecuteReader();
                if (reader.Read() && reader.GetBoolean(reader.GetOrdinal("IsAccepted")))
                {
                    return BadRequest("❌ Reason already accepted.");
                }
                reader.Close();

                // Insert clock-in (9:00 AM) and clock-out (5:00 PM) records
                var clockInTime = dto.Date.Date.AddHours(9); // 9:00 AM
                var clockOutTime = dto.Date.Date.AddHours(17); // 5:00 PM

                var insertCmd = new SqlCommand(
                    "INSERT INTO AttendanceLogs (UserId, Name, Timestamp, Status) VALUES (@UserId, @Name, @Timestamp, @Status)",
                    conn);

                // Clock-in
                insertCmd.Parameters.Clear();
                insertCmd.Parameters.AddWithValue("@UserId", dto.UserId);
                insertCmd.Parameters.AddWithValue("@Name", dto.Name);
                insertCmd.Parameters.AddWithValue("@Timestamp", clockInTime);
                insertCmd.Parameters.AddWithValue("@Status", 1);
                insertCmd.ExecuteNonQuery();

                // Clock-out
                insertCmd.Parameters.Clear();
                insertCmd.Parameters.AddWithValue("@UserId", dto.UserId);
                insertCmd.Parameters.AddWithValue("@Name", dto.Name);
                insertCmd.Parameters.AddWithValue("@Timestamp", clockOutTime);
                insertCmd.Parameters.AddWithValue("@Status", 0);
                insertCmd.ExecuteNonQuery();

                // Update LowHoursReasons to mark as accepted
                var updateCmd = new SqlCommand(
                    "UPDATE LowHoursReasons SET IsAccepted = 1 WHERE UserId = @UserId AND Date = @Date",
                    conn);
                updateCmd.Parameters.AddWithValue("@UserId", dto.UserId);
                updateCmd.Parameters.AddWithValue("@Date", dto.Date.Date);
                updateCmd.ExecuteNonQuery();

                return Ok(new { Message = $"✅ Reason accepted for {dto.Name} on {dto.Date:dd MMM yyyy}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"❌ Internal server error: {ex.Message}" });
            }
        }


        [HttpPost("submit-reason")]
        public IActionResult SubmitReason([FromBody] LowHoursReasonDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.UserId) || string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Reason))
                {
                    return BadRequest("❌ Invalid request data: UserId, Name, and Reason are required.");
                }

                var connStr = _config.GetConnectionString("DefaultConnection");
                using var conn = new SqlConnection(connStr);
                conn.Open();

                var cmd = new SqlCommand(
                    "INSERT INTO LowHoursReasons (UserId, Name, Date, Reason, CreatedAt) VALUES (@UserId, @Name, @Date, @Reason, @CreatedAt)",
                    conn);
                cmd.Parameters.AddWithValue("@UserId", dto.UserId);
                cmd.Parameters.AddWithValue("@Name", dto.Name);
                cmd.Parameters.AddWithValue("@Date", dto.Date.Date);
                cmd.Parameters.AddWithValue("@Reason", dto.Reason);
                cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

                cmd.ExecuteNonQuery();
                return Ok(new { Message = $"✅ Reason submitted for {dto.Date:dd MMM yyyy}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"❌ Internal server error: {ex.Message}" });
            }
        }
    }

    public class AttendanceDto
    {
        public string UserId { get; set; } = "";
        public string Name { get; set; } = "";
        public DateTime Timestamp { get; set; }
        public int Status { get; set; }
    }

    public class LowHoursReasonDto
    {
        public string UserId { get; set; } = "";
        public string Name { get; set; } = "";
        public DateTime Date { get; set; }
        public string Reason { get; set; } = "";
        public bool IsAccepted { get; set; } = false;
    }
    public class AcceptReasonDto
    {
        public string UserId { get; set; } = "";
        public string Name { get; set; } = "";
        public DateTime Date { get; set; }
    }
}