using ApplicationTracker.Models;
using ApplicationTracker.Repositories;
using ApplicationTracker.Utilities;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace ApplicationTracker.View_Models
{
    public class DayViewModel : ViewModelBase
    {
        public DayViewModel()
        {
            DailyTotal = new ObservableCollection<ProcessWrapper>(GetDailyTotal());
        }

        public ObservableCollection<ProcessWrapper> DailyTotal { get; set; }

        public IEnumerable<ProcessWrapper> GetDailyTotal()
        {
            IUnitOfWork uow = new UnitOfWork(new TrackContext());

            DateTime today = DateTime.Today;

            var dailyTotal = uow.ProcessTable.Find(p => p.ProcessDate.Date == today);

            return ViewModelUtils.ConvertProcDataToProcWrapper(dailyTotal);
        }

        private ObservableCollection<PieSeries<double>>? _pieSeries;

        public ObservableCollection<PieSeries<double>>? PieSeries
        {
            get => _pieSeries;

            set
            {
                _pieSeries = value;
                OnPropertyChanged();
            }
        }


        private List<ProcessWrapper> GetPieData(IEnumerable<ProcessWrapper> processes)
        {
            return processes
                .Where(p => p.ProcessTime.TotalMinutes > 2)
                .OrderByDescending(p => p.ProcessTime)
                .Take(10)
                .ToList();
        }

        private double GetTotalMinutes(IEnumerable<ProcessWrapper> processes)
        {
            return processes.Sum(p => p.ProcessTime.TotalMinutes);
        }

        private SKColor[] GetColorPalette()
        {
            return new SKColor[]
            {
                SKColors.CornflowerBlue,
                SKColors.LightGreen,
                SKColors.LightSalmon,
                SKColors.LightSkyBlue,
                SKColors.PaleGoldenrod,
                SKColors.PaleGreen,
                SKColors.PaleTurquoise,
                SKColors.PeachPuff,
                SKColors.Plum,
                SKColors.SandyBrown,
                SKColors.SkyBlue,
                SKColors.Tan,
                SKColors.Thistle,
                SKColors.Wheat,
                SKColors.Aquamarine,
                SKColors.Coral,
                SKColors.DarkSeaGreen,
                SKColors.HotPink,
                SKColors.Khaki,
                SKColors.MediumAquamarine,
                SKColors.MediumSlateBlue
            };
        }

        private List<(int Hours, int Minutes, int Seconds)> GetDurations(IEnumerable<ProcessWrapper> pieData)
        {
            return pieData
                .Select(proc => (proc.ProcessTime.Hours, proc.ProcessTime.Minutes, proc.ProcessTime.Seconds))
                .ToList();
        }

        private ObservableCollection<PieSeries<double>> CreatePieSlices(List<ProcessWrapper> pieData, List<(int Hours, int Minutes, int Seconds)> durations)
        {
            var pieSlices = new ObservableCollection<PieSeries<double>>();
            SKColor[] colors = GetColorPalette();
            double totalMinutes = GetTotalMinutes(pieData);

            for (int i = 0; i < pieData.Count; ++i)
            {
                // We perform the lookups here for better tooltip performance
                var process = pieData[i];
                var duration = durations[i];

                pieSlices.Add(new PieSeries<double>
                {
                    Name = process.ProcessName,
                    Values = new[] { process.ProcessTime.TotalMinutes / totalMinutes },
                    InnerRadius = 80,
                    Fill = new SolidColorPaint(colors[i]),
                    DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                    DataLabelsSize = 13,
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                    DataLabelsFormatter = point => $"{point.PrimaryValue:P2}",
                    ToolTipLabelFormatter = point => $"{point.PrimaryValue:P2} - {duration.Hours} Hours, {duration.Minutes} Minutes, {duration.Seconds} Seconds",
                });
            }

            return pieSlices;
        }

        private PieSeries<double> CreateOtherSlice(IEnumerable<ProcessWrapper> allProcesses, List<ProcessWrapper> pieData)
        {
            double totalMinutes = GetTotalMinutes(allProcesses);
            SKColor[] colors = GetColorPalette();
            var totalPercent = pieData.Sum(p => p.ProcessTime.TotalMinutes / totalMinutes);

            return new PieSeries<double>
            {
                Name = "Other",
                Values = new[] { 1 - totalPercent },
                InnerRadius = 80,
                Fill = new SolidColorPaint(colors[13]),
                DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                DataLabelsSize = 11,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                DataLabelsFormatter = point => $"{point.PrimaryValue:P2}",
                ToolTipLabelFormatter = point => $"{point.PrimaryValue:P2}",
            };
        }

        public ObservableCollection<PieSeries<double>> InitializePieChart(IEnumerable<ProcessWrapper> DailyTotal)
        {
            var pieData = GetPieData(DailyTotal);

            if (!pieData.Any())
                return new ObservableCollection<PieSeries<double>>();

            var durations = GetDurations(pieData);
            var piechart = CreatePieSlices(pieData, durations);

            if (GetTotalMinutes(DailyTotal) > 0)
                piechart.Add(CreateOtherSlice(DailyTotal, pieData));

            return piechart;
        }
    }
}
