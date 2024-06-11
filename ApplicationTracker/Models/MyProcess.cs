using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ApplicationTracker.Models
{
    public class MyProcess : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string ProcessName { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

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

        public TimeSpan PreviousProcessTime { get; set; }
        public DateTime ProcessDate { get; set; } // This property will store the date the process was run

        public MyProcess()
        {
            ProcessDate = DateTime.Now;
        }

    }
}
