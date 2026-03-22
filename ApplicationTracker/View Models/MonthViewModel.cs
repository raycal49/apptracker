using ApplicationTracker.Models;
using ApplicationTracker.Repositories;
using ApplicationTracker.Utilities;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ApplicationTracker.View_Models
{
    public class MonthViewModel : ViewModelBase
    {
        private readonly RelayCommand _nextMonthCommand;

        public MonthViewModel()
        {
            SelectedMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            PreviousMonthCommand = new RelayCommand(() => ChangeMonth(-1));
            _nextMonthCommand = new RelayCommand(() => ChangeMonth(1), () => CanMoveToNextMonth());
            NextMonthCommand = _nextMonthCommand;
            CurrentMonthCommand = new RelayCommand(() => GoToCurrentMonth(), () => !IsCurrentMonth);

            MonthlyTotal = new ObservableCollection<ProcessWrapper>();
            MonthlyTopProcesses = new ObservableCollection<ProcessWrapper>();
            DailyTotalsSeries = new ObservableCollection<ISeries>();
            TimeOfDaySeries = new ObservableCollection<ISeries>();
            DailyTotalsXAxes = CreateDayAxis();
            DailyTotalsYAxes = CreateHoursAxis("Hours");
            TimeOfDayXAxes = CreateHourAxis();
            TimeOfDayYAxes = CreateHoursAxis("Hours");

            Refresh(force: true);
        }

        private DateTime _selectedMonth;

        public DateTime SelectedMonth
        {
            get => _selectedMonth;
            private set
            {
                _selectedMonth = new DateTime(value.Year, value.Month, 1);
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedMonthLabel));
                OnPropertyChanged(nameof(IsCurrentMonth));
            }
        }

        public string SelectedMonthLabel => SelectedMonth.ToString("MMMM yyyy");

        public bool IsCurrentMonth => SelectedMonth.Year == DateTime.Today.Year && SelectedMonth.Month == DateTime.Today.Month;

        public ICommand PreviousMonthCommand { get; }

        public ICommand NextMonthCommand { get; }

        public ICommand CurrentMonthCommand { get; }

        private ObservableCollection<ProcessWrapper> _monthlyTotal;

        public ObservableCollection<ProcessWrapper> MonthlyTotal
        {
            get => _monthlyTotal;
            private set
            {
                _monthlyTotal = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ProcessWrapper> _monthlyTopProcesses;

        public ObservableCollection<ProcessWrapper> MonthlyTopProcesses
        {
            get => _monthlyTopProcesses;
            private set
            {
                _monthlyTopProcesses = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ISeries> _dailyTotalsSeries;

        public ObservableCollection<ISeries> DailyTotalsSeries
        {
            get => _dailyTotalsSeries;
            private set
            {
                _dailyTotalsSeries = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ISeries> _timeOfDaySeries;

        public ObservableCollection<ISeries> TimeOfDaySeries
        {
            get => _timeOfDaySeries;
            private set
            {
                _timeOfDaySeries = value;
                OnPropertyChanged();
            }
        }

        private Axis[] _dailyTotalsXAxes;

        public Axis[] DailyTotalsXAxes
        {
            get => _dailyTotalsXAxes;
            private set
            {
                _dailyTotalsXAxes = value;
                OnPropertyChanged();
            }
        }

        private Axis[] _dailyTotalsYAxes;

        public Axis[] DailyTotalsYAxes
        {
            get => _dailyTotalsYAxes;
            private set
            {
                _dailyTotalsYAxes = value;
                OnPropertyChanged();
            }
        }

        private Axis[] _timeOfDayXAxes;

        public Axis[] TimeOfDayXAxes
        {
            get => _timeOfDayXAxes;
            private set
            {
                _timeOfDayXAxes = value;
                OnPropertyChanged();
            }
        }

        private Axis[] _timeOfDayYAxes;

        public Axis[] TimeOfDayYAxes
        {
            get => _timeOfDayYAxes;
            private set
            {
                _timeOfDayYAxes = value;
                OnPropertyChanged();
            }
        }

        private TimeSpan _totalTrackedTime;

        public TimeSpan TotalTrackedTime
        {
            get => _totalTrackedTime;
            private set
            {
                _totalTrackedTime = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalTrackedTimeLabel));
            }
        }

        public string TotalTrackedTimeLabel => FormatDuration(TotalTrackedTime);

        private int _distinctAppsCount;

        public int DistinctAppsCount
        {
            get => _distinctAppsCount;
            private set
            {
                _distinctAppsCount = value;
                OnPropertyChanged();
            }
        }

        private string _topAppName = "None";

        public string TopAppName
        {
            get => _topAppName;
            private set
            {
                _topAppName = value;
                OnPropertyChanged();
            }
        }

        private TimeSpan _topAppTime;

        public TimeSpan TopAppTime
        {
            get => _topAppTime;
            private set
            {
                _topAppTime = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TopAppTimeLabel));
            }
        }

        public string TopAppTimeLabel => FormatDuration(TopAppTime);

        private string _busiestDayLabel = "No data";

        public string BusiestDayLabel
        {
            get => _busiestDayLabel;
            private set
            {
                _busiestDayLabel = value;
                OnPropertyChanged();
            }
        }

        private string _busiestHourLabel = "No data";

        public string BusiestHourLabel
        {
            get => _busiestHourLabel;
            private set
            {
                _busiestHourLabel = value;
                OnPropertyChanged();
            }
        }

        public void Refresh(bool force = false)
        {
            if (!force && !IsCurrentMonth)
            {
                return;
            }

            var monthlyEntries = new ObservableCollection<ProcessWrapper>(GetMonthlyTotal(SelectedMonth));

            MonthlyTotal = monthlyEntries;
            MonthlyTopProcesses = new ObservableCollection<ProcessWrapper>(GetTopProcesses(monthlyEntries));
            DailyTotalsSeries = BuildDailyTotalsSeries(monthlyEntries, SelectedMonth);
            TimeOfDaySeries = BuildTimeOfDaySeries(monthlyEntries);
            DailyTotalsXAxes = CreateDayAxis();
            DailyTotalsYAxes = CreateHoursAxis("Hours");
            TimeOfDayXAxes = CreateHourAxis();
            TimeOfDayYAxes = CreateHoursAxis("Hours");

            UpdateSummary(monthlyEntries);
            UpdateCommandStates();
        }

        private IEnumerable<ProcessWrapper> GetMonthlyTotal(DateTime selectedMonth)
        {
            using IUnitOfWork uow = new UnitOfWork(new TrackContext());

            var monthlyTotal = uow.ProcessTable.Find(p => p.ProcessDate.Month == selectedMonth.Month
                                                      && p.ProcessDate.Year == selectedMonth.Year);

            return ViewModelUtils.ConvertProcDataToProcWrapper(monthlyTotal);
        }

        private IEnumerable<ProcessWrapper> GetTopProcesses(IEnumerable<ProcessWrapper> monthlyProcesses)
        {
            return monthlyProcesses
                .GroupBy(p => p.ProcessName)
                .Select(g => new ProcessWrapper
                {
                    ProcessName = g.Key,
                    ProcessTime = new TimeSpan(g.Sum(p => p.ProcessTime.Ticks)),
                    ProcessDate = SelectedMonth
                })
                .OrderByDescending(p => p.ProcessTime)
                .Take(10)
                .ToList();
        }

        private ObservableCollection<ISeries> BuildDailyTotalsSeries(IEnumerable<ProcessWrapper> monthlyProcesses, DateTime selectedMonth)
        {
            int daysInMonth = DateTime.DaysInMonth(selectedMonth.Year, selectedMonth.Month);
            double[] dailyUsage = new double[daysInMonth];

            foreach (var dailyTotal in monthlyProcesses
                .GroupBy(p => p.ProcessDate.Date)
                .Select(g => new
                {
                    Day = g.Key.Day,
                    Hours = g.Sum(p => p.ProcessTime.TotalHours)
                }))
            {
                dailyUsage[dailyTotal.Day - 1] = dailyTotal.Hours;
            }

            return new ObservableCollection<ISeries>
            {
                new ColumnSeries<double>
                {
                    Name = "Daily Totals",
                    Values = dailyUsage,
                    Fill = new SolidColorPaint(SKColors.CornflowerBlue),
                    MaxBarWidth = 28
                }
            };
        }

        private ObservableCollection<ISeries> BuildTimeOfDaySeries(IEnumerable<ProcessWrapper> monthlyProcesses)
        {
            double[] hourlyUsage = new double[24];

            foreach (var hourTotal in monthlyProcesses
                .GroupBy(p => p.ProcessDate.Hour)
                .Select(g => new
                {
                    Hour = g.Key,
                    Hours = g.Sum(p => p.ProcessTime.TotalHours)
                }))
            {
                hourlyUsage[hourTotal.Hour] = hourTotal.Hours;
            }

            return new ObservableCollection<ISeries>
            {
                new ColumnSeries<double>
                {
                    Name = "Time of Day",
                    Values = hourlyUsage,
                    Fill = new SolidColorPaint(SKColors.MediumSlateBlue),
                    MaxBarWidth = 20
                }
            };
        }

        private Axis[] CreateDayAxis()
        {
            int daysInMonth = DateTime.DaysInMonth(SelectedMonth.Year, SelectedMonth.Month);

            return new Axis[]
            {
                new Axis
                {
                    Name = "Day",
                    Labels = Enumerable.Range(1, daysInMonth).Select(day => day.ToString()).ToArray(),
                    MinLimit = -0.5,
                    MaxLimit = daysInMonth - 0.5,
                    UnitWidth = 1,
                    TextSize = 12,
                    Padding = new Padding(4)
                }
            };
        }

        private Axis[] CreateHourAxis()
        {
            return new Axis[]
            {
                new Axis
                {
                    Name = "Hour",
                    Labels = Enumerable.Range(0, 24).Select(FormatHourLabel).ToArray(),
                    MinLimit = -0.5,
                    MaxLimit = 23.5,
                    UnitWidth = 1,
                    TextSize = 11,
                    LabelsRotation = 0,
                    Padding = new Padding(4)
                }
            };
        }

        private Axis[] CreateHoursAxis(string name)
        {
            return new Axis[]
            {
                new Axis
                {
                    Name = name,
                    Labeler = value => string.Format("{0:N1}", value),
                    MinLimit = 0,
                    TextSize = 12,
                    NamePadding = new Padding(4)
                }
            };
        }

        private void UpdateSummary(IEnumerable<ProcessWrapper> monthlyEntries)
        {
            var entries = monthlyEntries.ToList();

            TotalTrackedTime = new TimeSpan(entries.Sum(p => p.ProcessTime.Ticks));
            DistinctAppsCount = entries.Select(p => p.ProcessName).Distinct().Count();

            var topProcess = GetTopProcesses(entries).FirstOrDefault();
            TopAppName = topProcess?.ProcessName ?? "No data";
            TopAppTime = topProcess?.ProcessTime ?? TimeSpan.Zero;

            var busiestDay = entries
                .GroupBy(p => p.ProcessDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Total = new TimeSpan(g.Sum(p => p.ProcessTime.Ticks))
                })
                .OrderByDescending(x => x.Total)
                .FirstOrDefault();

            BusiestDayLabel = busiestDay == null
                ? "No data"
                : $"{busiestDay.Date:MMM d} ({FormatDuration(busiestDay.Total)})";

            var busiestHour = entries
                .GroupBy(p => p.ProcessDate.Hour)
                .Select(g => new
                {
                    Hour = g.Key,
                    Total = new TimeSpan(g.Sum(p => p.ProcessTime.Ticks))
                })
                .OrderByDescending(x => x.Total)
                .FirstOrDefault();

            BusiestHourLabel = busiestHour == null
                ? "No data"
                : $"{FormatHourRange(busiestHour.Hour)} ({FormatDuration(busiestHour.Total)})";
        }

        private void ChangeMonth(int monthOffset)
        {
            SelectedMonth = SelectedMonth.AddMonths(monthOffset);
            Refresh(force: true);
        }

        private void GoToCurrentMonth()
        {
            SelectedMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            Refresh(force: true);
        }

        private bool CanMoveToNextMonth()
        {
            var nextMonth = SelectedMonth.AddMonths(1);
            var currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            return nextMonth <= currentMonth;
        }

        private void UpdateCommandStates()
        {
            _nextMonthCommand.RaiseCanExecuteChanged();

            if (CurrentMonthCommand is RelayCommand currentMonthCommand)
            {
                currentMonthCommand.RaiseCanExecuteChanged();
            }
        }

        private static string FormatDuration(TimeSpan duration)
        {
            int totalHours = (int)duration.TotalHours;
            return $"{totalHours}h {duration.Minutes}m";
        }

        private static string FormatHourLabel(int hour)
        {
            return DateTime.Today.Date.AddHours(hour).ToString("htt").ToLowerInvariant();
        }

        private static string FormatHourRange(int hour)
        {
            string start = DateTime.Today.Date.AddHours(hour).ToString("htt").ToLowerInvariant();
            string end = DateTime.Today.Date.AddHours((hour + 1) % 24).ToString("htt").ToLowerInvariant();
            return $"{start}–{end}";
        }
    }
}
