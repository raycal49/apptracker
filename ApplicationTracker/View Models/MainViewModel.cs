
namespace ApplicationTracker.View_Models
{
    public class MainViewModel : ViewModelBase
    {

        public MainViewModel()
        {
            DayVm = new DayViewModel();
            WeekVm = new WeekViewModel();
            MonthVm = new MonthViewModel();
        }

        public DayViewModel DayVm { get; set; }
        public WeekViewModel WeekVm { get; set; }
        public MonthViewModel MonthVm { get; set; }
    }
}
