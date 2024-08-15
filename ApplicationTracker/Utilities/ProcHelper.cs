using ApplicationTracker.Models;
using ApplicationTracker.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker.Utilities
{
    public class ProcHelper
    {
        public void ProcTimer(HashSet<string> exclusionList, ObservableCollection<ProcessWrapper> runningProcs, IdleDetect idleDetect, ActiveWindow active)
        {
            //ActiveWindowHelper helper = new ActiveWindowHelper();    

            //Process[] processes = Process.GetProcesses();

            GetRunningProcs(exclusionList, runningProcs);

            IdleTimeInfo idleTimeInfo = idleDetect.GetIdleTimeInfo(new IdleDetectHelper());

            if (idleTimeInfo.IdleTime.TotalMinutes <= 1)
            {
                //IActiveWindowHelper helper = new ActiveWindowHelper();
                UpdateRunningProcs(runningProcs, active);
            }

        }

        public virtual void GetRunningProcs(HashSet<string> exclusionList, ObservableCollection<ProcessWrapper> runningProcs)
        {
            Process[] processes = Process.GetProcesses();

            foreach (Process item in processes)
            {

                if (item.MainWindowHandle != IntPtr.Zero
                                          && !exclusionList.Contains(item.ProcessName)
                                          && !runningProcs.Any(p => p.ProcessName == item.ProcessName))
                {
                    runningProcs.Add(new ProcessWrapper() { ProcessName = item.ProcessName });
                }

            }
        }

        public void UpdateProcessTable(ObservableCollection<ProcessWrapper> currentSessionsRunningProcs)
        {
            IUnitOfWork uow = new UnitOfWork(new TrackContext());

            DateTime today = DateTime.Today;

            foreach (ProcessWrapper proc in currentSessionsRunningProcs)
            {
                var dbProcess = uow.ProcessTable.FindFirstOrDef(p => p.ProcessName == proc.ProcessName && p.ProcessDate.Date == proc.ProcessDate.Date);

                if (dbProcess != null)
                {
                    dbProcess.ProcessTime = proc.ProcessTime;
                }
                else if (dbProcess == null && proc.ProcessDate.Date == today)
                {
                    uow.ProcessTable.Add(new ProcessData() { ProcessName = proc.ProcessName, ProcessTime = proc.ProcessTime, ProcessDate = proc.ProcessDate });
                }
            }
            uow.Complete();
        }

        public virtual void UpdateRunningProcs(ObservableCollection<ProcessWrapper> runningProcs, ActiveWindow window)
        {

            foreach (ProcessWrapper proc in runningProcs)
            {
                if (window.IsActive(proc.ProcessName))
                {
                    TimeSpan interval = TimeSpan.FromSeconds(1);
                    proc.ProcessTime += interval;
                    break;
                }
            }
        }
    }
}
