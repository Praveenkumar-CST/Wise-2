namespace AttendanceBackend.Model
{
    public class AttendanceLog
    {
        public int Id { get; set; }                    // Primary Key
        public int UserId { get; set; }                // ZKTeco user ID
        public DateTime Timestamp { get; set; }        // Date and time of attendance
        public string Status { get; set; }          // Optional (e.g. "CheckIn", "CheckOut")
    }
}
