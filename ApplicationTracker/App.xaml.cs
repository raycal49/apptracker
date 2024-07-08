using ApplicationTracker;
using ApplicationTracker.Models;
using ApplicationTracker.View_Models;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace ApplicationTimerApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ObservableCollection<MyProcess> myProcesses = new ObservableCollection<MyProcess>();
            // need to refactor this to accept *just* an interface later. this'll do for now.
            MyProcessViewModel processVM = new MyProcessViewModel(myProcesses);

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

            MainWindow = new MainWindow()
            {
                DataContext = processVM
            };
            MainWindow.Show();

            DispatcherTimer pTimer = new DispatcherTimer();
            pTimer.Tick += (s, e) => processVM.ProcTimer(exclusionList, myProcesses);
            pTimer.Interval = TimeSpan.FromSeconds(1);
            pTimer.Start();

            //processVM.ProcTimer(exclusionList, processVM.MyProcessCollection);

            System.Timers.Timer dayTimer = new System.Timers.Timer(15000);
            dayTimer.Elapsed += (s, e) => processVM.DailyCount(myProcesses);
            dayTimer.AutoReset = true;
            dayTimer.Enabled = true;

            //processVM.DailyTimer(processVM.MyProcessCollection);

            base.OnStartup(e);
        }
    }

}
