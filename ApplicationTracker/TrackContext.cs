using ApplicationTracker.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker
{
    internal class TrackContext : DbContext
    {
        public DbSet<MyProcess> Processes { get; set; }
        public DbSet<DailyTotal> DailyTotals { get; set; }
        public DbSet<WeeklyTotal> WeeklyTotals { get; set; }
        public DbSet<MonthlyTotal> MonthlyTotals { get; set; }
        public DbSet<YearlyTotal> YearlyTotals { get; set; }

        public TrackContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                   "Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = TrackDatabase"
                   );
        }
    }
}
