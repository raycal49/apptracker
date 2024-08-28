using Microsoft.EntityFrameworkCore;

namespace ApplicationTracker.Models
{
    internal class TrackContext : DbContext
    {
        public DbSet<ProcessData> ProcessTable { get; set; }

        public TrackContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = TrackDatabase");
        }
    }
}
