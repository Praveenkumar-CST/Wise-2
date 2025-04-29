using Microsoft.EntityFrameworkCore;
using CalendarApi.Models;

namespace CalendarApi.Data
{
    public class CalendarDbContext : DbContext
    {
        public CalendarDbContext(DbContextOptions<CalendarDbContext> options) : base(options) { }

        public DbSet<Event> Events { get; set; }
        public DbSet<SavedHoliday> SavedHolidays { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Date)
                    .IsRequired()
                    .HasMaxLength(10);
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.HolidayType)
                    .IsRequired()
                    .HasMaxLength(10);
                entity.Property(e => e.Day)
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<SavedHoliday>(entity =>
            {
                entity.HasKey(sh => new { sh.UserEmail, sh.Date });
                entity.Property(sh => sh.UserEmail)
                    .IsRequired()
                    .HasMaxLength(255); // Standard length for email addresses
                entity.Property(sh => sh.Date)
                    .IsRequired()
                    .HasMaxLength(10);
                entity.Property(sh => sh.EventName)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}