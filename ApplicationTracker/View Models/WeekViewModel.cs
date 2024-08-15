using ApplicationTracker.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker.View_Models
{
    public class WeekViewModel
    {
        public ObservableCollection<ProcessWrapper>? WeeklyTotal { get; set; }
    }
}
