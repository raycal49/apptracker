using ApplicationTracker.Models;
using ApplicationTracker.Repositories;
using ApplicationTracker.Utilities;
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
            DailyTotal = new ObservableCollection<ProcessWrapper>(GetDailyTotal());
        }

        public ObservableCollection<ProcessWrapper> DailyTotal { get; set; }

        public IEnumerable<ProcessWrapper> GetDailyTotal()
        {
            IUnitOfWork uow = new UnitOfWork(new TrackContext());

            DateTime today = DateTime.Today;

            var dailyTotal = uow.ProcessTable.Find(p => p.ProcessDate.Date == today);

            return ViewModelUtils.ConvertProcDataToProcWrapper(dailyTotal);
        }
    }

  
}
