using ApplicationTracker.Models;
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
        public void ProcTimer(HashSet<string> exclusionList, ObservableCollection<MyProcess> runningProcs, IdleDetect idleDetect, ActiveWindow active)
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

        public virtual void GetRunningProcs(HashSet<string> exclusionList, ObservableCollection<MyProcess> runningProcs)
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

        public virtual void UpdateRunningProcs(ObservableCollection<MyProcess> runningProcs, ActiveWindow window)
        {

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
    }
}
