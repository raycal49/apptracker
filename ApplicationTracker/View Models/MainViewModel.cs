
namespace ApplicationTracker.View_Models
{
    public class MainViewModel : ViewModelBase
    {

        public MainViewModel()
        {
            DayVm = new DayViewModel();
            WeekVm = new WeekViewModel();
        }

        public DayViewModel DayVm { get; set; }
        public WeekViewModel WeekVm { get; set; }
    }
}
