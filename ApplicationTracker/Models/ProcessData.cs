using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker.Models
{
    public class ProcessData
    {
        public int Id { get; set; }
        public required string ProcessName { get; set; }
        public TimeSpan ProcessTime { get; set; }
        public DateTime ProcessDate { get; set; }
    }
}
