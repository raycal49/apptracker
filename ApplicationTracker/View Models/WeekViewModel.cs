using ApplicationTracker.Models;
using ApplicationTracker.Repositories;
using ApplicationTracker.Utilities;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker.View_Models
{
    public class WeekViewModel : ViewModelBase
    {
        public WeekViewModel()
        {
            WeeklyTotal = new ObservableCollection<ProcessWrapper>(GetWeeklyTotal());
        }

        public ObservableCollection<ProcessWrapper>? WeeklyTotal { get; set; }

        private ObservableCollection<ISeries>? _weeklyChartSeries;

        public ObservableCollection<ISeries>? WeeklyChartSeries
        {
            get { return _weeklyChartSeries; }
            set
            {
                _weeklyChartSeries = value;
                OnPropertyChanged();
            }
        }

        private IEnumerable<ProcessWrapper> GetWeeklyTotal()
        {
            IUnitOfWork uow = new UnitOfWork(new TrackContext());

            CultureInfo myCI = new CultureInfo("en-US");
            Calendar myCal = myCI.Calendar;

            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;

            var pTable = uow.ProcessTable.GetAll().ToList();

            IEnumerable<ProcessData> weeklyTotal = pTable.FindAll(p =>
                                                         myCal.GetWeekOfYear(p.ProcessDate.Date, myCWR, myFirstDOW) == myCal.GetWeekOfYear(DateTime.Now, myCWR, myFirstDOW)
                                                         && (p.ProcessDate.Year == DateTime.Now.Year));

            return ViewModelUtils.ConvertProcDataToProcWrapper(weeklyTotal);
        }

        private ObservableCollection<ProcessWrapper> GetChartData(IEnumerable<ProcessWrapper> weeklyProcesses)
        {
            return new ObservableCollection<ProcessWrapper>(
                weeklyProcesses
                    .GroupBy(p => p.ProcessDate.DayOfWeek)
                    .SelectMany(g =>
                        g.Where(p => p.ProcessTime > TimeSpan.FromMinutes(30))
                         .OrderByDescending(p => p.ProcessTime)
                         .Take(10)));
        }

        private SKColor[] GetColorPalette()
        {
            return new SKColor[]
            {
                SKColors.CornflowerBlue,
                SKColors.LightGreen,
                SKColors.LightSkyBlue,
                SKColors.PaleGoldenrod,
                SKColors.PaleGreen,
                SKColors.PaleTurquoise,
                SKColors.PeachPuff,
                SKColors.Plum,
                SKColors.SkyBlue,
                SKColors.Thistle,
                SKColors.Aquamarine,
                SKColors.DarkSeaGreen,
                SKColors.HotPink,
                SKColors.Khaki,
                SKColors.MediumAquamarine,
                SKColors.MediumSlateBlue
            };
        }
        private ObservableCollection<ISeries> CreateChart(ObservableCollection<ProcessWrapper> filteredProcesses)
        {
            var chart = new ObservableCollection<ISeries>();
            SKColor[] colors = GetColorPalette();

            int colorIndex = 0;
            int colorCount = colors.Length;


            foreach (var proc in filteredProcesses)
            {

                chart.Add(new StackedColumnSeries<double>
                {
                    Name = proc.ProcessName,
                    Values = new[] { proc.ProcessTime.TotalHours },
                    Fill = new SolidColorPaint(colors[colorIndex % colorCount]),
                    Mapping = (process, index) => new((int)proc.ProcessDate.DayOfWeek, process),
                    MaxBarWidth = 290,
                });

                colorIndex++;
            }

            return chart;
        }

        public ObservableCollection<ISeries> InitializeWeeklyChart(ObservableCollection<ProcessWrapper> weeklyProcesses)
        {
            var chartData = GetChartData(weeklyProcesses);

            if (!chartData.Any())
                return new ObservableCollection<ISeries>();

            var chart = CreateChart(chartData);

            return chart;
        }

    }
}
