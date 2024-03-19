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

namespace ApplicationTracker
{

    public partial class MainWindow : Window
    {
        internal List<MyProcess> runningProcesses = new List<MyProcess>();

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

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();

            void timer_Tick(object sender, EventArgs e)
            {
                Process[] processes = Process.GetProcesses();

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

        void SaveChanges()
        {
            using var _context = new TrackContext();

            Calendar myCal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;

            foreach (var proc in runningProcesses)
            {
                // dbProcess is either NULL or it is the process we have in our database with the desired process name

                var dbProcess = _context.Processes.FirstOrDefault(p => p.ProcessName == proc.ProcessName);

                // If we already have that specific process in the database, just increase its time by the time in runningProcesses.

                if (dbProcess != null && (DateTime.Compare(proc.DateStarted.Hour, dbProcess.DateStarted.Hour) > 1))
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
