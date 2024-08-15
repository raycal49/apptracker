using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker.View_Models
{
    public class MainViewModel : ViewModelBase
    {

        public MainViewModel()
        {
            DayVm = new DayViewModel();
            WeekVm = new WeekViewModel();
        }

        public DayViewModel DayVm { get; set; }
        public WeekViewModel WeekVm { get; set; }
    }
}
