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
using DetectLibrary;

namespace ApplicationTracker.View_Models
{
    public class MyProcessViewModel
    {

        public MyProcessViewModel(ObservableCollection<MyProcess> myProcessList)
        {
            MyProcessCollection = myProcessList;
        }

        public ObservableCollection<MyProcess> MyProcessCollection { get; set; }

        public void ProcTimer(HashSet<string> exclusionList, ObservableCollection<MyProcess> runningProcs)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();

            void timer_Tick(object sender, EventArgs e)
            {
                // Refactor GetRunningProcs to take in Process[].
                GetRunningProcs(exclusionList, runningProcs);

                var idleTime = IdleTimeDetector.GetIdleTimeInfo();

                if (idleTime.IdleTime.TotalMinutes <= 1)
                {
                    UpdateRunningProcs(runningProcs, timer.Interval);
                }
            }
        }

        public void GetRunningProcs(HashSet<string> exclusionList, ObservableCollection<MyProcess> runningProcs)
        {
            // Pass this through the function
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

        public void UpdateRunningProcs(ObservableCollection<MyProcess> runningProcs, TimeSpan interval)
        {
            foreach (MyProcess proc in runningProcs)
            {
                if (ProcessUtilities.IsActive(proc.ProcessName))
                {
                    proc.ProcessTime += interval;
                    break;
                }
            }
        }

        public void DailyTimer(ObservableCollection<MyProcess> runningProcs)
        {
            System.Timers.Timer timer = new System.Timers.Timer();

            //timer = new System.Timers.Timer(300000);
            timer = new System.Timers.Timer(15000);
            timer.Elapsed += (sender, e) => DailyCount(sender, e, runningProcs);
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        public void DailyCount(Object source, ElapsedEventArgs e, ObservableCollection<MyProcess> runningProcs)
        {
            using var _context = new TrackContext();

            DateTime today = DateTime.Today;

            foreach (MyProcess proc in runningProcs)
            {
                var dbProcess = _context.DailyTotals.FirstOrDefault(p => p.ProcessName == proc.ProcessName && p.ProcessDate.Date == proc.ProcessDate.Date);

                if (dbProcess != null)
                {
                    dbProcess.TotalTime += (proc.ProcessTime - proc.PreviousProcessTime);
                }
                else if (dbProcess == null && proc.ProcessDate.Date == today)
                {
                    _context.DailyTotals.Add(new DailyTotal() { ProcessName = proc.ProcessName, TotalTime = proc.ProcessTime, ProcessDate = proc.ProcessDate });
                }

                proc.PreviousProcessTime = proc.ProcessTime;
            }

            _context.SaveChanges();
        }
    }
}
