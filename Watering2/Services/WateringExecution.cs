using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using Watering2.Configuration;
using Watering2.Models;

namespace Watering2.Services
{
    public class WateringExecution : INotifyPropertyChanged, IDisposable
    {
        private ConfigController _cfgCtrl;
        private DataService _wateringDataService;

        private Timer _mainTimer = null;
        private Timer _eveningTimer = null;
        private Timer _mainPumpTimer = null;
        private Timer _eveningPumpTimer = null;
        private Timer _animationTimer = null;

        private const int AnimationUpdateTime = 5 * 60 * 1000; //alle 5 Minuten
        private const int AnimationStartTime = 10 * 1000;   //10 sec Startverzögerung

        private bool _debugMode;
        private bool _pumpsActive;

        //MainCycle: Morning, Secondary: Evening
        private enum WateringEvent { None, SecondaryCycle, MainCycle }

        private WateringEvent _wateringEvent = WateringEvent.None;


        public string TimeToWatering { get; private set; }
        public string TimeToWatering2 { get; private set; }

        public WateringExecution(ConfigController cfgController, DataService wateringDataService, bool debug)
        {
            _cfgCtrl = cfgController;
            _wateringDataService = wateringDataService;
            _debugMode = debug;
            _pumpsActive = false;

            _cfgCtrl.PropertyChanged += CfgController_PropertyChanged;

            StartClocks();
        }
        

        private void CfgController_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_pumpsActive)
                return;

            _mainTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _eveningTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            StartClocks();
        }

        private void StartClocks()
        {
            var mainDelay = CalculateTimeToMainWatering();
            if (_mainTimer == null)
                _mainTimer = new Timer(StartPrimaryWatering, null, mainDelay, Timeout.Infinite);
            else
                _mainTimer.Change(mainDelay, Timeout.Infinite);

            if (_cfgCtrl.Configuration.SecondWateringActive)
            {
                var eveningDelay = CalculateTimeToSecondWatering();
                if (_eveningTimer == null)
                    _eveningTimer = new Timer(StartSecondaryWatering, null, eveningDelay, Timeout.Infinite);
                else
                    _eveningTimer.Change(eveningDelay, Timeout.Infinite);
            }

            if (_animationTimer == null)
                _animationTimer = new Timer(UpdateInfos, null, AnimationStartTime, AnimationUpdateTime);
        }

        private int CalculateTimeToMainWatering()
        {
            var now = DateTime.Now;
            TimeSpan timeToWatering;
            if (now.TimeOfDay >= _cfgCtrl.Configuration.StartWateringMainCycle)
                timeToWatering = new TimeSpan(24, 0, 0) - now.TimeOfDay + _cfgCtrl.Configuration.StartWateringMainCycle;
            else
                timeToWatering = _cfgCtrl.Configuration.StartWateringMainCycle - now.TimeOfDay;

            TimeToWatering = $"{timeToWatering.Hours} Stunden {timeToWatering.Minutes} Minuten {timeToWatering.Seconds} Sekunden";
            OnPropertyChanged(nameof(TimeToWatering));

            return
                (timeToWatering.Hours * 60 * 60 + timeToWatering.Minutes * 60 + timeToWatering.Seconds) * 1000;
        }

        private int CalculateTimeToSecondWatering()
        {
            var now = DateTime.Now;
            TimeSpan timeToWatering2;
            if (now.TimeOfDay >= _cfgCtrl.Configuration.StartWateringSecondCycle)
                timeToWatering2 = new TimeSpan(24, 0, 0) - now.TimeOfDay + _cfgCtrl.Configuration.StartWateringSecondCycle;
            else
                timeToWatering2 = _cfgCtrl.Configuration.StartWateringSecondCycle - now.TimeOfDay;

            TimeToWatering2 = $"{timeToWatering2.Hours} Stunden {timeToWatering2.Minutes} Minuten {timeToWatering2.Seconds} Sekunden";
            OnPropertyChanged(nameof(TimeToWatering2));


            return
                (timeToWatering2.Hours * 60 * 60 + timeToWatering2.Minutes * 60 + timeToWatering2.Seconds) * 1000;
        }

        public WateringCorrection CalcWateringTimeForStatistics(DateTime? requDate)
        {
            WateringCorrection wCorr = CalcWateringDurationCorrection(requDate);
            wCorr.WateringDuration =  _cfgCtrl.Configuration.PumpDurationMainCycle * wCorr.CorrFactorHot; //sec
            return wCorr;
        }

        private WateringCorrection CalcWateringDurationCorrection(DateTime? requDate)
        {
            //Terminal.Settings.DisplayLoggingMessageType = LogMessageType.Info | LogMessageType.Warning | LogMessageType.Error | LogMessageType.Fatal | LogMessageType.Debug;

            bool statistics = requDate.HasValue;

            DateTime yesterdayBegin;
            DateTime yesterdayBeginAfternoon;
            DateTime yesterdayEnd;
            DateTime endForRain;

            bool emergencyWatering = _wateringDataService.EmergencyWateringNecessary(_cfgCtrl.Configuration.MaxDaysWithoutWatering);

            var now = requDate ?? DateTime.Today;

            if (statistics)
            {
                yesterdayBegin = now.Add(_cfgCtrl.Configuration.BeginMonitoring);
                yesterdayBeginAfternoon = now.Add(new TimeSpan(13, 0, 0));
                yesterdayEnd = now.Add(_cfgCtrl.Configuration.EndMonitoring);
                endForRain = now.AddHours(23).AddMinutes(59).AddSeconds(59);
            }
            else
            {
                var yesterday = now.AddDays(-1);
                yesterdayBegin = yesterday.Add(_cfgCtrl.Configuration.BeginMonitoring);
                yesterdayBeginAfternoon = yesterday.Add(new TimeSpan(13, 0, 0));
                yesterdayEnd = yesterday.Add(_cfgCtrl.Configuration.EndMonitoring);
                endForRain = DateTime.Now;
            }

            List<Measurement> samples =
                _wateringDataService.GetReadingPointsByStartAndEndTime(yesterdayBegin, yesterdayEnd);
            
            int samplesCnt = samples.Count;

            List<Measurement> samplesAfternoon =
                _wateringDataService.GetReadingPointsByStartAndEndTime(yesterdayBeginAfternoon, yesterdayEnd);

            List<Measurement> samplesForRain =
                _wateringDataService.GetReadingPointsByStartAndEndTime(endForRain.AddHours(-24), endForRain);

            int cntRain = samplesForRain.Count(p => p.Raining);
            int durationRain = cntRain * _cfgCtrl.Configuration.MeasurementFrequency;

            int stopDuration = _cfgCtrl.Configuration.RainDurationToStopWatering.Hours * 60 * 60 + 
                               _cfgCtrl.Configuration.RainDurationToStopWatering.Minutes * 60 +
                               _cfgCtrl.Configuration.RainDurationToStopWatering.Seconds;

            bool stopWateringBecauseOfRain = durationRain >= stopDuration;

            if (stopWateringBecauseOfRain)
            {
                //$"Keine Bewässerung: {durationRain} sec über {stopDuration} sec".Warn("CalcWateringDurationCorrection", $"start: {(endForRain.AddHours(-24)):d/M/yy hh:mm} end: {endForRain:d/M/yy hh:mm}", string.Empty);
                if (emergencyWatering)
                {
                    //"Not Bewässerung wird ausgeführt".Warn();
                }
            }

            int hotSamplesCntAfternoon = samplesAfternoon.Count(point => point.Temperature >= _cfgCtrl.Configuration.LevelHeatTemperature);

            double tempAboveHotLevelAfternoon = 0d;
            foreach (Measurement sample in samplesAfternoon)
            {
                var diff = sample.Temperature - _cfgCtrl.Configuration.LevelHeatTemperature;
                if (diff > 0)
                    tempAboveHotLevelAfternoon += diff;
            }

            //Durchschnitts Übertemperatur 40° soll Korrekturwert _corrFactorHot ergeben
            double corrAdditive = (_cfgCtrl.Configuration.CorrFactorHeat - 1d) / (40d - _cfgCtrl.Configuration.LevelHeatTemperature); 
            double corrHotNew = hotSamplesCntAfternoon > 0 ? 
                1d + (tempAboveHotLevelAfternoon / hotSamplesCntAfternoon) * ((double)hotSamplesCntAfternoon / samplesAfternoon.Count) * corrAdditive 
                : 1;

            if (corrHotNew < 1d)
            {
                //$"CorrHot korrigiert: Errechnter Wert: {corrHotNew} ".Warn();
                corrHotNew = 1d;
            }

            if (statistics)
                return new WateringCorrection() { CorrFactorHot = stopWateringBecauseOfRain ? emergencyWatering ? 1 : 0 : corrHotNew, RainDuration = durationRain };

            var wateringData = new WateringData()
            {
                TimeStamp = DateTime.Now,
                CorrCold = 1,
                CorrHot = corrHotNew,
                SamplesCount = samplesCnt,
                SamplesHot = hotSamplesCntAfternoon,
                SamplesCold = 0,
                Duration = stopWateringBecauseOfRain ? emergencyWatering ? 1 : 0 : _cfgCtrl.Configuration.PumpDurationMainCycle * corrHotNew,
                NoWateringBecauseRain = stopWateringBecauseOfRain,
                EmergencyWatering = emergencyWatering,
                PercentageHot = samplesAfternoon.Count > 0 ? (double)hotSamplesCntAfternoon/samplesAfternoon.Count * 100d : 0d,
                PercentageCold = 0d,
                DurationRain = durationRain
            };

            _wateringDataService.SaveWateringData(wateringData);
            return new WateringCorrection()
            {
                CorrFactorHot = (stopWateringBecauseOfRain ? 0 : corrHotNew),
                RainDuration = durationRain
            };
        }


        public bool IsSecondWateringNecessaryForStatistics(DateTime? requDate)
        {
            return IsSecondWateringNecessary(requDate);
        }

        private bool IsSecondWateringNecessary(DateTime? requDate)
        {
            //Terminal.Settings.DisplayLoggingMessageType = LogMessageType.Info | LogMessageType.Warning | LogMessageType.Error | LogMessageType.Fatal | LogMessageType.Debug;

            bool forStatistics = requDate.HasValue;

            if (!_cfgCtrl.Configuration.SecondWateringActive && !forStatistics)
                return false;

            var now = requDate ?? DateTime.Today;
            DateTime nowBeginAfternoon = now.Add(new TimeSpan(13, 0, 0));
            DateTime nowEnd = now.Add(_cfgCtrl.Configuration.EndMonitoring);

            List<Measurement> samples =
                _wateringDataService.GetReadingPointsByStartAndEndTime(nowBeginAfternoon, nowEnd);

            int samplesCntAfternoon = samples.Count;

            int hotSamplesCnt = samples.Count(point => point.Temperature >= _cfgCtrl.Configuration.LevelHeatTemperature);

            double percentHot = samplesCntAfternoon > 0 ? (double)hotSamplesCnt / samplesCntAfternoon : 0d;

            percentHot *= 100d;

            if (forStatistics)
            {
                //"Leaving For Statistics".Warn();
                return percentHot >=_cfgCtrl.Configuration.PercentageHotFor2ndWateringActive;
            }

            if (percentHot < _cfgCtrl.Configuration.PercentageHotFor2ndWateringActive)
            {
                TimeToWatering2 = $"Kein 2. Gießen: {percentHot:F1}% über {_cfgCtrl.Configuration.LevelHeatTemperature:##}°";
                //$"Kein 2. Gießen: {percentHot:F1}% über {_levelHot:##}°".Warn("IsSecondWateringNecessary", $"start: {nowBeginAfternoon:d/M/yy hh:mm} end: {nowEnd:d/M/yy hh:mm}", string.Empty);

                OnPropertyChanged(nameof(TimeToWatering2));
                return false;
            }

            //$"2. Gießen: {percentHot:F1}% über {_cfgCtrl.Configuration.LevelHeatTemperature:##}°".Info("IsSecondWateringNecessary", $"start: {nowBeginAfternoon:d/M/yy hh:mm} end: {nowEnd:d/M/yy hh:mm}", string.Empty);

            var traceData = new WateringData()
            {
                TimeStamp = DateTime.Now,
                CorrCold = 1,
                CorrHot = 1,
                SamplesCount = samplesCntAfternoon,
                SamplesHot = hotSamplesCnt,
                SamplesCold = 0,
                Duration = _cfgCtrl.Configuration.PumpDurationSecondCycle,
            };

            _wateringDataService.SaveWateringData(traceData);

            return true;
        }

        private void StartPrimaryWatering(object state)
        {
            int duration = Convert.ToInt32(_cfgCtrl.Configuration.PumpDurationMainCycle * CalcWateringDurationCorrection(null).CorrFactorHot * 1000);  //msec
            if (duration == 0)
            {
                StopPrimaryWatering(null);
                //"Bewässerung wurde nicht ausgelöst: Korr: 0".Warn();
                return;
            }

            _pumpsActive = true;
            _wateringEvent = WateringEvent.SecondaryCycle;

            if (!_debugMode)
            {
                DigitalIOConnector.Instance.TurnOnPump();
            }

            if (_mainPumpTimer == null)
                _mainPumpTimer = new Timer(StopPrimaryWatering, null, duration, Timeout.Infinite);
            else
                _mainPumpTimer.Change(duration, Timeout.Infinite);
            UpdateInfos(null);
        }

        private void StopPrimaryWatering(object o)
        {
            if (!_debugMode) DigitalIOConnector.Instance.TurnOffPump();
            var delay = CalculateTimeToMainWatering();
            _mainTimer.Change(delay, Timeout.Infinite);
            _pumpsActive = false;
            _wateringEvent = WateringEvent.None;
            UpdateInfos(null);
        }

        private void StartSecondaryWatering(object state)
        {
            //$"Entering StartEveningWatering: active: {_secondWateringActive} default duration: {_wateringDuration2}".Info();
            if (!_cfgCtrl.Configuration.SecondWateringActive)
                return;

            if (!IsSecondWateringNecessary(null))
            {
                StopSecondaryWatering(null);
                return;
            }

            _pumpsActive = true;
            _wateringEvent = WateringEvent.MainCycle;
            int duration = Convert.ToInt32(_cfgCtrl.Configuration.PumpDurationSecondCycle * 1000);
            if (!_debugMode)
            {
                DigitalIOConnector.Instance.TurnOnPump();
            }

            if (_eveningPumpTimer == null)
                _eveningPumpTimer = new Timer(StopSecondaryWatering, null, duration, Timeout.Infinite);
            else
                _eveningPumpTimer.Change(duration, Timeout.Infinite);
            UpdateInfos(null);
        }

        private void StopSecondaryWatering(object state)
        {
            var delay = CalculateTimeToSecondWatering();
            //$"Entering StopEveningWatering: next watering: {delay}".Info();
            if (!_debugMode) DigitalIOConnector.Instance.TurnOffPump();
            _eveningTimer.Change(delay, Timeout.Infinite);
            _pumpsActive = false;
            _wateringEvent = WateringEvent.None;
            UpdateInfos(null);
        }

        private void UpdateInfos(object state)
        {
            if (_pumpsActive)
            {
                if (_wateringEvent == WateringEvent.SecondaryCycle)
                {
                    TimeToWatering = "Es wird gegossen";
                    OnPropertyChanged(nameof(TimeToWatering));
                }
                else if (_wateringEvent == WateringEvent.MainCycle)
                {
                    TimeToWatering2 = "Es wird gegossen";
                    OnPropertyChanged(nameof(TimeToWatering2));
                }
                return;
            }
            CalculateTimeToMainWatering();
            CalculateTimeToSecondWatering();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            _mainTimer?.Dispose();
            _eveningTimer?.Dispose();
            _mainPumpTimer?.Dispose();
            _eveningPumpTimer?.Dispose();
            _animationTimer?.Dispose();
            if (_cfgCtrl != null)
                _cfgCtrl.PropertyChanged -= CfgController_PropertyChanged;
            if (!_debugMode) DigitalIOConnector.Instance.TurnOffPump();
        }
    }
}
