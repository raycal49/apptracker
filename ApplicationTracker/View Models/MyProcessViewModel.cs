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
using IdleDetect;

namespace ApplicationTracker.View_Models
{
    public class MyProcessViewModel
    {

        public MyProcessViewModel()
        {
            MyProcessCollection = new ObservableCollection<MyProcess>();
        }

        public ObservableCollection<MyProcess> MyProcessCollection { get; set; }

        public void ProcTimer(HashSet<string> exclusionList, ObservableCollection<MyProcess> runningProcs)
        {
                //ActiveWindowHelper helper = new ActiveWindowHelper();    

                //Process[] processes = Process.GetProcesses();

                GetRunningProcs(exclusionList, runningProcs);

                var idleTime = IdleTimeDetect.GetIdleTimeInfo(new IdleDetectHelper());

                if (idleTime.IdleTime.TotalMinutes <= 1)
                {
                    UpdateRunningProcs(runningProcs);
                }
            
        }

        public void GetRunningProcs(HashSet<string> exclusionList, ObservableCollection<MyProcess> runningProcs)
        {
            Process[] processes = Process.GetProcesses();

            foreach (Process item in processes)
            {

                if (item.MainWindowHandle != IntPtr.Zero
                                          && !exclusionList.Contains(item.ProcessName)
                                          && !runningProcs.Any(p => p.ProcessName == item.ProcessName))
                {
                    runningProcs.Add(new MyProcess() { ProcessName = item.ProcessName });
                }

            }
        }

        public void UpdateRunningProcs(ObservableCollection<MyProcess> runningProcs)
        {
            ActiveWindow window = new ActiveWindow(new ActiveWindowHelper());

            foreach (MyProcess proc in runningProcs)
            {
                if (window.IsActive(proc.ProcessName))
                {
                    TimeSpan interval = TimeSpan.FromSeconds(1);
                    proc.ProcessTime += interval;
                    break;
                }
            }
        }

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
