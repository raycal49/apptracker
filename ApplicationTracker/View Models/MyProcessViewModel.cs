using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;
using ApplicationTracker.Models;
using ApplicationTracker.Repositories;
using ApplicationTracker.Utilities;

namespace ApplicationTracker.View_Models
{
    public class MyProcessViewModel
    {

        public MyProcessViewModel()
        {
            MyProcessCollection = new ObservableCollection<MyProcess>();
        }

        public ObservableCollection<MyProcess> MyProcessCollection { get; set; }

        public void DailyCount(ObservableCollection<MyProcess> runningProcs)
        {
            IUnitOfWork uow = new UnitOfWork(new TrackContext());
            //using var _context = new TrackContext();

            DateTime today = DateTime.Today;

            foreach (MyProcess proc in runningProcs)
            {
                var dbProcess = uow.DailyTotals.FindFirstOrDef(p => p.ProcessName == proc.ProcessName && p.ProcessDate.Date == proc.ProcessDate.Date);

                if (dbProcess != null)
                {
                    dbProcess.TotalTime += (proc.ProcessTime - proc.PreviousProcessTime);
                }
                else if (dbProcess == null && proc.ProcessDate.Date == today)
                {
                    uow.DailyTotals.Add(new DailyTotal() { ProcessName = proc.ProcessName, TotalTime = proc.ProcessTime, ProcessDate = proc.ProcessDate });
                }

                proc.PreviousProcessTime = proc.ProcessTime;
            }

            uow.Complete();
        }
    }
}
