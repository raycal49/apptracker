using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using ApplicationTracker.Models;
using System.Windows.Threading;
using System.Globalization;
using System.ComponentModel;
using Calendar = System.Globalization.Calendar;
using System.Timers;
using System.Configuration;
using System.Collections.ObjectModel;
using ApplicationTracker.View_Models;

namespace ApplicationTracker
{
    public partial class MainWindow : Window
    {
        //internal ObservableCollection<MyProcess> currentMyProcesses = new ObservableCollection<MyProcess>();
        //MyProcessViewModel processVM = new MyProcessViewModel(new ObservableCollection<MyProcess>());

        public MainWindow()
        {
            InitializeComponent();
            /*
            HashSet<string> exclusionList = new HashSet<string>
            {
                //"explorer",
                "textinputhost",
                "ApplicationFrameHost",
                "svchost",
                //"devenv",
                "TextInputHost",
                "updatechecker",
            };
            */

            //DataContext = processVM;
            //ProcessInfo.ItemsSource = processVM._myprocess;
            //ProcessInfo.ItemsSource = processVM.MyProcess;

            //ProcTimer();
            //DailyTimer();

            /*
            void ProcTimer()
            {
                DispatcherTimer timer = new DispatcherTimer();
                timer.Tick += new EventHandler(timer_Tick);
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Start();

                void timer_Tick(object sender, EventArgs e)
                {
                    GetRunningProcs();

                    var idleTime = IdleTimeDetector.GetIdleTimeInfo();

                    if (idleTime.IdleTime.TotalMinutes <= 1)
                    {
                        UpdateRunningProcs(processVM.MyProcess, timer.Interval);
                    }
                }
            }
            */
            /*
            void DailyTimer()
            {
                System.Timers.Timer timer = new System.Timers.Timer();

                //timer = new System.Timers.Timer(300000);
                timer = new System.Timers.Timer(15000);
                timer.Elapsed += OnTimedEvent;
                timer.AutoReset = true;
                timer.Enabled = true;
            }
            */
            /*
            void GetRunningProcs()
            {
                Process[] processes = Process.GetProcesses();

                foreach (Process item in processes)
                {

                    if (item.MainWindowHandle != IntPtr.Zero
                                              && !exclusionList.Contains(item.ProcessName)
                                              && !processVM.MyProcess.Any(p => p.ProcessName == item.ProcessName))
                    {
                        processVM.MyProcess.Add(new MyProcess() { ProcessName = item.ProcessName });
                    }

                }
            }
            */
            /*
            void UpdateRunningProcs(ObservableCollection<MyProcess> processes, TimeSpan interval)
            {
                foreach (MyProcess proc in processVM.MyProcess)
                {
                    if (ProcessUtilities.IsActive(proc.ProcessName))
                    {
                        proc.ProcessTime += interval;
                        break;
                    }
                }
            }
            */
            
            /*
            void OnTimedEvent(Object source, ElapsedEventArgs e)
            {
                DailyCount();
            }

            void DailyCount()
            {
                using var _context = new TrackContext();

                DateTime today = DateTime.Today;

                foreach (MyProcess proc in processVM.MyProcess)
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
            */
        }
        
        // reinstate the SaveChanges() function at some point.
        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you want to save the process data before exiting?",
                                               "Save Data",
                                               MessageBoxButton.YesNoCancel,
                                               MessageBoxImage.Question);
            try
            {
                if (result == MessageBoxResult.Yes)
                {
                    //SaveChanges();
                    Console.WriteLine("Hi");
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    // Cancel the closing event
                    e.Cancel = true;
                }
                // If result is No, do nothing and close the application
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
        
        /*
        void SaveChanges()
        {
            using var _context = new TrackContext();

            //DateTime today = DateTime.Today;
            //int week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(today, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            //int month = today.Month;
            //int year = today.Year;

            // Determine current session's usage. Definitely need to extract to a function
            foreach (MyProcess proc in processVM.MyProcess)
            {
                // dbProcess is either NULL or it is the process we have in our database with the desired process name
                // this is the "local" session application tracking. We will reset the local session every time the program closes.
                var dbProcess = _context.Processes.FirstOrDefault(p => p.ProcessName == proc.ProcessName);

                // If we already have that specific process in the database, just increase its time by the time in runningProcesses.
                if (dbProcess != null)
                {
                    // If process exists, update it.
                    dbProcess.ProcessTime += proc.ProcessTime;
                }
                else
                {
                    // If process does not exist, add it.
                    _context.Processes.Add(proc);
                }
            }

            _context.SaveChanges();
        }
        */
    }

}
