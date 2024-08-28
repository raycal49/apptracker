
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
