using ApplicationTracker.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker.View_Models
{
    public class DayViewModel : ViewModelBase
    {
        public DayViewModel()
        {
            DailyTotal = new ObservableCollection<ProcessWrapper>();
        }

        public ObservableCollection<ProcessWrapper> DailyTotal { get; set; }
    }

  
}
