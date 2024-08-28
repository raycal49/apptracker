using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ApplicationTracker.Models
{
    public class ProcessWrapper : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string ProcessName { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public TimeSpan _processtime;

        public TimeSpan ProcessTime
        {
            get => _processtime;
            set
            {
                if (value != _processtime)
                {
                    _processtime = value;
                    OnPropertyChanged(nameof(ProcessTime));
                }
            }

        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public DateTime ProcessDate { get; set; }

        public ProcessWrapper()
        {
            ProcessDate = DateTime.Now;
        }
    }
}
