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
using DetectLibrary;
using ApplicationTracker.Models;
using System.Windows.Threading;
using System.Globalization;
using System.ComponentModel;
using Calendar = System.Globalization.Calendar;
using System.Timers;

namespace ApplicationTracker
{

    public partial class MainWindow : Window
    {
        internal List<MyProcess> runningProcesses = new List<MyProcess>();
        Process[] processes = Process.GetProcesses();

        public MainWindow()
        {
            InitializeComponent();

            HashSet<string> exclusionList = new HashSet<string>
            {
                //"explorer",
                "textinputhost",
                "ApplicationFrameHost",
                "svchost",
                "devenv",
                "TextInputHost",
                "updatechecker",
            };

            // Definitely need to extract everything to individual functions and whatnot.

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
            DailyTimer();

            void timer_Tick(object sender, EventArgs e)
            {

                // Grab running processes and put them in our runningProcesses list, according to our conditionals.
                foreach (Process item in processes)
                {

                    if (item.MainWindowHandle != IntPtr.Zero && !exclusionList.Contains(item.ProcessName) && !runningProcesses.Any(p => p.ProcessName == item.ProcessName))
                    {
                        runningProcesses.Add(new MyProcess() { ProcessName = item.ProcessName });
                    }

                    //ProcessInfo.Items.Refresh();
                }

                var idleTime = IdleTimeDetector.GetIdleTimeInfo();

                if (idleTime.IdleTime.TotalMinutes <= 1)
                {
                    foreach (MyProcess proc in runningProcesses)
                    {

                        if (ProcessUtilities.IsActive(proc.ProcessName))
                        {
                            proc.ProcessTime += timer.Interval;
                            break;
                        }

                        ProcessInfo.Items.Refresh();
                    }

                }
            }
            ProcessInfo.ItemsSource = runningProcesses;
            
            //DailyTimer();

            // Add to Daily Total every 5 minutes.
            void DailyTimer()
            {
                System.Timers.Timer timer = new System.Timers.Timer();

                timer = new System.Timers.Timer(300000);
                timer.Elapsed += OnTimedEvent;
                timer.AutoReset = true;
                timer.Enabled = true;
            }

            void OnTimedEvent(Object source, ElapsedEventArgs e)
            {
                DailyCount();
                Console.WriteLine("The Elapsed event was raised at {0}", e.SignalTime);
            }
            


        }

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
                    SaveChanges();
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

        // Need to execute DailyCount upon exiting, too
        void DailyCount()
        {
            using var _context = new TrackContext();

            DateTime today = DateTime.Today;

            foreach (var proc in runningProcesses)
            {
                // dbProcess is either NULL or it is the process we have in our database with the desired process name
                // this is the daily application tracking session. We will store all instances of our "local" application tracking into this daily.
                var dbProcess = _context.DailyTotals.FirstOrDefault(p => p.ProcessName == proc.ProcessName && proc.ProcessDate.Date == today);

                // If we already have that specific process in the database, just increase its time by the time in runningProcesses.
                if (dbProcess != null)
                {
                    // Need to modify how we store daily time as currently it doesn't work correctly.
                    dbProcess.TotalTime += proc.ProcessTime;
                }
                else
                {
                    // If process does not exist, add it.
                    _context.DailyTotals.Add(new DailyTotal () { ProcessName = proc.ProcessName, TotalTime = proc.ProcessTime, ProcessDate = proc.ProcessDate});
                }
            }

            _context.SaveChanges();


        }

        void SaveChanges()
        {
            using var _context = new TrackContext();

            //DateTime today = DateTime.Today;
            //int week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(today, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            //int month = today.Month;
            //int year = today.Year;

            // Determine current session's usage. Definitely need to extract to a function
            foreach (var proc in runningProcesses)
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

    }

}
