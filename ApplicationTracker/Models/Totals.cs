using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker.Models
{
    internal class DailyTotal
    {
        public int Id { get; set; }
        public string ProcessName { get; set; }
        public TimeSpan TotalTime { get; set; }
        public DateTime ProcessDate { get; set; }

        //Key for DailyTotal should really be ProcessName + ProcessDate

        //  foreign key stuff
        //public int WeeklyTotalId { get; set; }
        //public WeeklyTotal WeeklyTotal { get; set; }

    }
    internal class WeeklyTotal
    {
        public int Id { get; set; }
        public string ProcessName { get; set; } //fk
        public TimeSpan TotalTime { get; set; }
        public int Week { get; set; }
        public int Year { get; set; }


        //public ICollection<DailyTotal> DailyTotals { get; set; }

    }
    internal class MonthlyTotal
    {
        public int Id { get; set; }
        public string ProcessName { get; set; } //fk
        public TimeSpan TotalTime { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

    }
    public class YearlyTotal
    {
        public int Id { get; set; }
        public string ProcessName { get; set; } //fk
        public TimeSpan TotalTime { get; set; }
        public int Year { get; set; }
    }
}
