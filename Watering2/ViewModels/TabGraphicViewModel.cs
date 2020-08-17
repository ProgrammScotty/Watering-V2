using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using System.Text;
using System.Linq;
using ReactiveUI;
using Watering2.Configuration;
using Watering2.Models;
using Watering2.Services;
using Watering2.Utils;
using Point = Avalonia.Point;

namespace Watering2.ViewModels
{
    public class TabGraphicViewModel : ViewModelBase
    {
        private DataService _dataService;
        private WateringExecution _wateringTimers;
        private ConfigController _cfgController;

        const int _canvasWidth = 800;
        private const double _canvasHeight = 270;

        public TabGraphicViewModel()
        {
            _selectedDate = DateTime.Now.AddDays(-1);
        }

        public TabGraphicViewModel(ConfigController cfgController, DataService service, WateringExecution wateringTimers)
        {
            _dataService = service;
            _wateringTimers = wateringTimers;
            _cfgController = cfgController;

            SelDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0); //AddDays(-1);

            CalcWateringTimeCmd = ReactiveCommand.CreateFromTask(async unit => { DoCalculateWateringTimeAndSecondWatering(); });
        }

        public ReactiveCommand<Unit, Unit> CalcWateringTimeCmd { get; }

        private List<Measurement> _readingPoints = new List<Measurement>();
        public List<Measurement> ReadingPoints
        {
            get => _readingPoints;
            set => this.RaiseAndSetIfChanged(ref _readingPoints, value);
        }


        private List<Point> _temperaturePlot = new List<Point>() { new Point(0, 0), new Point(30, 30), new Point(60, -60) };
        public List<Point> TemperaturePlot
        {
            get => _temperaturePlot;
            set => this.RaiseAndSetIfChanged(ref _temperaturePlot, value);
        }

        private List<Point> _humidityPlot = new List<Point>() { new Point(0, 60), new Point(30, 80), new Point(60, 50) };
        public List<Point> HumidityPlot
        {
            get => _humidityPlot;
            set => this.RaiseAndSetIfChanged(ref _humidityPlot, value);
        }


        private string _minTempForPlot = "0°";
        public string MinTempForPlot
        {
            get => _minTempForPlot;
            set => this.RaiseAndSetIfChanged(ref _minTempForPlot, value);
        }

        private string _maxTempForPlot = "45°";
        public string MaxTempForPlot
        {
            get => _maxTempForPlot;
            set => this.RaiseAndSetIfChanged(ref _maxTempForPlot, value);
        }

        private string _minHumForPlot = "30%";
        public string MinHumForPlot
        {
            get => _minHumForPlot;
            set => this.RaiseAndSetIfChanged(ref _minHumForPlot, value);
        }

        private string _maxHumForPlot = "99.9%";
        public string MaxHumForPlot
        {
            get => _maxHumForPlot;
            set => this.RaiseAndSetIfChanged(ref _maxHumForPlot, value);
        }

        private string _minTimeForPlot = "00:00:00";
        public string MinTimeForPlot
        {
            get => _minTimeForPlot;
            set => this.RaiseAndSetIfChanged(ref _minTimeForPlot, value);
        }

        private string _middleTimeForPlot = "12:00:00";
        public string MiddleTimeForPlot
        {
            get => _middleTimeForPlot;
            set => this.RaiseAndSetIfChanged(ref _middleTimeForPlot, value);
        }

        private string _maxTemperatPosForPlot = "200";
        public string MaxTemperatPosForPlot
        {
            get => _maxTemperatPosForPlot;
            set => this.RaiseAndSetIfChanged(ref _maxTemperatPosForPlot, value);
        }

        private string _minTemperatPosForPlot = "200";
        public string MinTemperatPosForPlot
        {
            get => _minTemperatPosForPlot;
            set => this.RaiseAndSetIfChanged(ref _minTemperatPosForPlot, value);
        }

        private string _maxHumidityPosForPlot = "200";
        public string MaxHumidityPosForPlot
        {
            get => _maxHumidityPosForPlot;
            set => this.RaiseAndSetIfChanged(ref _maxHumidityPosForPlot, value);
        }

        private string _maxHumidityTimeForPlot = "04:00";
        public string MaxHumidityTimeForPlot
        {
            get => _maxHumidityTimeForPlot;
            set => this.RaiseAndSetIfChanged(ref _maxHumidityTimeForPlot, value);
        }

        private string _maxTimeForPlot = "23:59:59";
        public string MaxTimeForPlot
        {
            get => _maxTimeForPlot;
            set => this.RaiseAndSetIfChanged(ref _maxTimeForPlot, value);
        }

        private string _maxTemperatTimeForPlot = "14:00:00";
        public string MaxTemperatTimeForPlot
        {
            get => _maxTemperatTimeForPlot;
            set => this.RaiseAndSetIfChanged(ref _maxTemperatTimeForPlot, value);
        }

        private string _minTemperatTimeForPlot = "08:00:00";
        public string MinTemperatTimeForPlot
        {
            get => _minTemperatTimeForPlot;
            set => this.RaiseAndSetIfChanged(ref _minTemperatTimeForPlot, value);
        }

        private string _middleTimePosForPlot = (_canvasWidth/2).ToString();
        public string MiddleTimePosForPlot
        {
            get => _middleTimePosForPlot;
            set => this.RaiseAndSetIfChanged(ref _middleTimePosForPlot, value);
        }

        private bool _noDataForPlot = true;
        public bool NoDataForPlot
        {
            get => _noDataForPlot;
            set => this.RaiseAndSetIfChanged(ref _noDataForPlot, value);
        }

        private DateTime? _selectedDate = DateTime.Today;
        public DateTime? SelDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                UpdateReadings();
            }
        }


        private string _calculatedWateringDuration = "0";
        public string CalculatedWateringDuration
        {
            get => _calculatedWateringDuration;
            set => this.RaiseAndSetIfChanged(ref _calculatedWateringDuration, value);
        }

        private bool _isSecondWateringNecessary = false;
        public bool IsSecondWateringNecessary
        {
            get => _isSecondWateringNecessary;
            set => this.RaiseAndSetIfChanged(ref _isSecondWateringNecessary, value);
        }

        private string _rainDuration = "0:0:0";
        public string RainDuration
        {
            get => _rainDuration;
            set => this.RaiseAndSetIfChanged(ref _rainDuration, value);
        }

        private string _corrCold = "1";
        public string CorrCold
        {
            get => _corrCold;
            set => this.RaiseAndSetIfChanged(ref _corrCold, value);
        }

        private Point _hotLimitStartPoint = new Point(0, 0);
        public Point HotLimitStartPoint
        {
            get => _hotLimitStartPoint;
            set => this.RaiseAndSetIfChanged(ref _hotLimitStartPoint, value);
        }

        private Point _hotLimitEndPoint = new Point(_canvasWidth, 0);
        public Point HotLimitEndPoint
        {
            get => _hotLimitEndPoint;
            set => this.RaiseAndSetIfChanged(ref _hotLimitEndPoint, value);
        }

        private bool _hotLimitLineVisible = true;
        public bool HotLimitLineVisible
        {
            get => _hotLimitLineVisible;
            set => this.RaiseAndSetIfChanged(ref _hotLimitLineVisible, value);
        }

        private string _hotLimitValuePos = "20";
        public string HotLimitValuePos
        {
            get => _hotLimitValuePos;
            set => this.RaiseAndSetIfChanged(ref _hotLimitValuePos, value);
        }

        private string _levelHotValue = "35°";
        public string LevelHotValue
        {
            get => _levelHotValue;
            set => this.RaiseAndSetIfChanged(ref _levelHotValue, value);
        }

        public void UpdateReadings()
        {
            DateTime startTime = _selectedDate.GetValueOrDefault(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0));
            List<Measurement> tempList = _dataService.GetReadingPointsByStartAndEndTime(startTime, startTime.AddDays(1));

            if (tempList == null || tempList.Count <= 3)
            {
                NoDataForPlot = true;
                return;
            }

            NoDataForPlot = false;

            double maxTemperat = tempList.Max(x => x.Temperature);
            double minTemperat = tempList.Min(x => x.Temperature);
            double maxHum = tempList.Max(x => x.Humidity);
            double minHum = tempList.Min(x => x.Humidity);

            double tempRange = maxTemperat - minTemperat;
            double tempScaleFactor = -((_canvasHeight-2)/2) / (tempRange > 0 ? tempRange : 1); // Canvas.Top

            double levelHot = _cfgController.Configuration.LevelHeatTemperature;

            if (maxTemperat > levelHot)
            {
                HotLimitLineVisible = true;
                HotLimitStartPoint = new Point(0, (levelHot - minTemperat) * tempScaleFactor);
                HotLimitEndPoint = new Point(_canvasWidth, HotLimitStartPoint.Y);
                HotLimitValuePos = ((int)(134d + HotLimitStartPoint.Y)).ToString(); //134: Nullpunkt der Temperaturkurve aus xaml file
                LevelHotValue = $"{levelHot:##} °";
            }
            else
            {
                HotLimitLineVisible = false;
            }

            //(readingPt.Temperature - minTemperat) * tempScaleFactor)

            double humRange = maxHum - minHum;
            double humScaleFactor = -((_canvasHeight-2)/2) / (humRange > 0 ? humRange : 1);

            int cntReadings = tempList.Count;

            int toSkip = 0;
            double pixPerReading = 1;


            if (_canvasWidth < cntReadings)
            {
                toSkip = cntReadings / (int)_canvasWidth;
            }
            else
            {
                pixPerReading = (double)_canvasWidth / (cntReadings > 1 ? cntReadings : cntReadings);
            }


            TimeSpan timeRange = (tempList[cntReadings - 1].TimeStamp - tempList[0].TimeStamp) / 2;
            TimeSpan middleTime = timeRange.Add(new TimeSpan(tempList[0].TimeStamp.Hour, tempList[0].TimeStamp.Minute, tempList[0].TimeStamp.Second));
            int middleIntTime = middleTime.Seconds + middleTime.Minutes * 100 + middleTime.Hours * 10000;

            List<Point> tmpTemperaturePoints = new List<Point>();
            List<Point> tmpHumidityPoints = new List<Point>();

            double actColumn = 0;
            var skippedItems = 0;
            double posMaxTemperat = 0;
            double posMinTemperat = 0;
            double posMaxHumidity = 0;
            DateTime maxTemperatTime = startTime;
            DateTime minTemperatTime = startTime;
            DateTime maxHumidityTime = startTime;
            var minDiffToMiddleTime = Int32.MaxValue;
            double posMiddleTime = 0;
            string strgMiddleTime = String.Empty;

            foreach (Measurement readingPt in tempList)
            {
                if (Math.Abs(readingPt.Temperature - maxTemperat) <= 0.15d)
                {
                    posMaxTemperat = pixPerReading * actColumn;
                    maxTemperatTime = readingPt.TimeStamp;
                }
                if (Math.Abs(readingPt.Temperature - minTemperat) <= 0.15d)
                {
                    posMinTemperat = pixPerReading * actColumn;
                    minTemperatTime = readingPt.TimeStamp;
                }
                if (Math.Abs(readingPt.Humidity - maxHum) <= 0.15d)
                {
                    posMaxHumidity = pixPerReading * actColumn;
                    maxHumidityTime = readingPt.TimeStamp;
                }

                var actTime = readingPt.TimeStamp.Second + readingPt.TimeStamp.Minute * 100 + readingPt.TimeStamp.Hour * 10000;
                if (Math.Abs(actTime - middleIntTime) < minDiffToMiddleTime)
                {
                    minDiffToMiddleTime = (Math.Abs(actTime - middleIntTime));
                    posMiddleTime = pixPerReading * actColumn;
                    strgMiddleTime = readingPt.TimeStamp.ToString("HH:mm");
                }

                if (toSkip != 0)
                {
                    skippedItems++;
                    if (skippedItems < toSkip)
                        continue;
                    skippedItems = 0;
                }

                tmpTemperaturePoints.Add(new Point((int)(pixPerReading * actColumn), (readingPt.Temperature - minTemperat) * tempScaleFactor));
                tmpHumidityPoints.Add(new Point(pixPerReading * actColumn, (readingPt.Humidity - minHum) * humScaleFactor));
                actColumn++;
            }

            ReadingPoints.Clear();
            ReadingPoints = null;
            ReadingPoints = tempList;

            TemperaturePlot.Clear();
            TemperaturePlot = null;
            TemperaturePlot = tmpTemperaturePoints;

            HumidityPlot.Clear();
            HumidityPlot = null;
            HumidityPlot = tmpHumidityPoints;

            MaxTempForPlot = maxTemperat.ToString("F1") + " °";
            MinTempForPlot = minTemperat.ToString("F1") + " °";

            MaxHumForPlot = maxHum.ToString("F1") + " %";
            MinHumForPlot = minHum.ToString("F1") + "%";

            MinTimeForPlot = tempList[0].TimeStamp.ToString("HH:mm");
            MaxTimeForPlot = tempList[^1].TimeStamp.ToString("HH:mm");

            MaxTemperatPosForPlot = ((int)posMaxTemperat).ToString();
            MaxTemperatTimeForPlot = maxTemperatTime.ToString("HH:mm");

            MinTemperatPosForPlot = ((int)posMinTemperat).ToString();
            MinTemperatTimeForPlot = minTemperatTime.ToString("HH:mm");

            MaxHumidityPosForPlot = ((int)posMaxHumidity).ToString();
            MaxHumidityTimeForPlot = maxHumidityTime.ToString("HH:mm");

            MiddleTimeForPlot = strgMiddleTime;
            MiddleTimePosForPlot = ((int)posMiddleTime).ToString();
        }

        private void DoCalculateWateringTimeAndSecondWatering()
        {
            WateringCorrection wCorr = _wateringTimers.CalcWateringTimeForStatistics(_selectedDate);
            TimeSpan ts = TimeSpan.FromSeconds(wCorr.WateringDuration);
            CalculatedWateringDuration = ts.Minutes + "m" + ts.Seconds + "s";

            TimeSpan rn = TimeSpan.FromSeconds(wCorr.RainDuration);

            RainDuration = $"{rn.Hours}:{rn.Minutes}:{rn.Seconds}";

            CorrCold = wCorr.CorrFactorCold.ToString("##.##");

            IsSecondWateringNecessary = _wateringTimers.IsSecondWateringNecessaryForStatistics(_selectedDate);
        }

    }
}
