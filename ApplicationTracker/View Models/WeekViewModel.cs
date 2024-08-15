using ApplicationTracker.Models;
using ApplicationTracker.Repositories;
using ApplicationTracker.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker.View_Models
{
    public class WeekViewModel
    {
        public WeekViewModel()
        {
            WeeklyTotal = new ObservableCollection<ProcessWrapper>(GetWeeklyTotal());
        }

        public ObservableCollection<ProcessWrapper>? WeeklyTotal { get; set; }

        private IEnumerable<ProcessWrapper> GetWeeklyTotal()
        {
            IUnitOfWork uow = new UnitOfWork(new TrackContext());

            CultureInfo myCI = new CultureInfo("en-US");
            Calendar myCal = myCI.Calendar;

            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;

            var pTable = uow.ProcessTable.GetAll().ToList();

            IEnumerable<ProcessData> weeklyTotal = pTable.FindAll(p =>
                                                         myCal.GetWeekOfYear(p.ProcessDate.Date, myCWR, myFirstDOW) == myCal.GetWeekOfYear(DateTime.Now, myCWR, myFirstDOW)
                                                         && (p.ProcessDate.Year == DateTime.Now.Year));

            return ViewModelUtils.ConvertProcDataToProcWrapper(weeklyTotal);
        }
    }
}
