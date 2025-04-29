namespace CalendarApi.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string? Day { get; set; }
        public string Description { get; set; }
        public string HolidayType { get; set; }
    }
}
