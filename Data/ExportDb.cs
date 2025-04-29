using System.Collections.Generic;
using AttendanceBackend.Model;
using Microsoft.EntityFrameworkCore;

namespace AttendanceBackend.Data // Adjust namespace to match your project
{
    public class YourDbContext : DbContext
    {
        public YourDbContext(DbContextOptions<YourDbContext> options) : base(options)
        {
        }
        public DbSet<LowHoursReason> LowHoursReasons { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LowHoursReason>()
                .HasIndex(l => new { l.UserId, l.Date })
                .HasDatabaseName("IX_LowHoursReasons_UserId_Date");
        }
    }

    public class AttendanceRecord
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public DateTime Timestamp { get; set; }
        public int Status { get; set; }
    }
    
}