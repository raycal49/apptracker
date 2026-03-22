using ApplicationTracker.Models;
using ApplicationTracker.Repositories;
using ApplicationTracker.Utilities;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace ApplicationTracker.View_Models
{
    public class MonthViewModel : ViewModelBase
    {
        public MonthViewModel()
        {
            MonthlyTotal = new ObservableCollection<ProcessWrapper>(GetMonthlyTotal());
            MonthlyTopProcesses = new ObservableCollection<ProcessWrapper>(GetTopProcesses(MonthlyTotal));
            MonthlyChartSeries = InitializeMonthlyChart(MonthlyTotal);
            XAxes = CreateXAxis();
            YAxes = CreateYAxis();
        }

        public ObservableCollection<ProcessWrapper> MonthlyTotal { get; set; }

        public ObservableCollection<ProcessWrapper> MonthlyTopProcesses { get; set; }

        private ObservableCollection<ISeries>? _monthlyChartSeries;

        public ObservableCollection<ISeries>? MonthlyChartSeries
        {
            get => _monthlyChartSeries;
            set
            {
                _monthlyChartSeries = value;
                OnPropertyChanged();
            }
        }

        private Axis[]? _xAxes;

        public Axis[]? XAxes
        {
            get => _xAxes;
            set
            {
                _xAxes = value;
                OnPropertyChanged();
            }
        }

        private Axis[]? _yAxes;

        public Axis[]? YAxes
        {
            get => _yAxes;
            set
            {
                _yAxes = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<ProcessWrapper> GetMonthlyTotal()
        {
            using IUnitOfWork uow = new UnitOfWork(new TrackContext());

            DateTime today = DateTime.Today;

            var monthlyTotal = uow.ProcessTable.Find(p => p.ProcessDate.Month == today.Month
                                                      && p.ProcessDate.Year == today.Year);

            return ViewModelUtils.ConvertProcDataToProcWrapper(monthlyTotal);
        }

        public IEnumerable<ProcessWrapper> GetTopProcesses(IEnumerable<ProcessWrapper> monthlyProcesses)
        {
            return monthlyProcesses
                .GroupBy(p => p.ProcessName)
                .Select(g => new ProcessWrapper
                {
                    ProcessName = g.Key,
                    ProcessTime = new TimeSpan(g.Sum(p => p.ProcessTime.Ticks)),
                    ProcessDate = DateTime.Today
                })
                .OrderByDescending(p => p.ProcessTime)
                .Take(10)
                .ToList();
        }

        public ObservableCollection<ISeries> InitializeMonthlyChart(IEnumerable<ProcessWrapper> monthlyProcesses)
        {
            DateTime today = DateTime.Today;
            int daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);
            double[] dailyUsage = new double[daysInMonth];

            var dailyTotals = monthlyProcesses
                .GroupBy(p => p.ProcessDate.Date)
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    Day = g.Key.Day,
                    Hours = g.Sum(p => p.ProcessTime.TotalHours)
                })
                .ToList();

            if (!dailyTotals.Any())
            {
                return new ObservableCollection<ISeries>();
            }

            foreach (var dailyTotal in dailyTotals)
            {
                dailyUsage[dailyTotal.Day - 1] = dailyTotal.Hours;
            }

            return new ObservableCollection<ISeries>
            {
                new ColumnSeries<double>
                {
                    Name = "Daily Usage",
                    Values = dailyUsage,
                    Fill = new SolidColorPaint(SKColors.CornflowerBlue)
                }
            };
        }

        public Axis[] CreateXAxis()
        {
            DateTime today = DateTime.Today;
            int daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);

            return new Axis[]
            {
                new Axis
                {
                    Name = "Day",
                    Labels = Enumerable.Range(1, daysInMonth).Select(day => day.ToString()).ToArray(),
                    MinLimit = -0.5,
                    MaxLimit = daysInMonth - 0.5,
                    UnitWidth = 1,
                    TextSize = 14,
                    LabelsRotation = 0,
                    Padding = new Padding(4)
                }
            };
        }

        public Axis[] CreateYAxis()
        {
            return new Axis[]
            {
                new Axis
                {
                    Name = "Hours",
                    Labeler = value => string.Format("{0:N1}", value),
                    MinLimit = 0,
                    TextSize = 14,
                    NamePadding = new Padding(4)
                }
            };
        }

        public void Refresh()
        {
            MonthlyTotal = new ObservableCollection<ProcessWrapper>(GetMonthlyTotal());
            MonthlyTopProcesses = new ObservableCollection<ProcessWrapper>(GetTopProcesses(MonthlyTotal));
            MonthlyChartSeries = InitializeMonthlyChart(MonthlyTotal);
            XAxes = CreateXAxis();
            YAxes = CreateYAxis();
            OnPropertyChanged(nameof(MonthlyTotal));
            OnPropertyChanged(nameof(MonthlyTopProcesses));
        }
    }
}
