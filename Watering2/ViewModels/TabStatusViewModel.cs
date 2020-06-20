using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using System.Text;
using ReactiveUI;
using Watering2.Configuration;
using Watering2.Models;
using Watering2.Services;
using Watering2.Utils;

namespace Watering2.ViewModels
{
    public class TabStatusViewModel : ViewModelBase
    {
        private WeatherSensorDataProvider _sensorDataProvider;
        private ConfigController _cfgCtrl;
        private WateringExecution _wateringExecution;
        private bool _debug;


        public RangeAddableObservableCollection<Measurement> LastReadingPoints { get; }
        public ReactiveCommand<Unit, Unit> ChangePumpStatusCmd { get; }

        //for design time
        public TabStatusViewModel()
        {

        }

        public TabStatusViewModel(ConfigController cfgController, WeatherSensorDataProvider sensorDataProvider, WateringExecution wateringExecution, bool debug)
        {
            _sensorDataProvider = sensorDataProvider;
            _cfgCtrl = cfgController;
            _wateringExecution = wateringExecution;
            _debug = debug;

            _wateringExecution.PropertyChanged += WateringExecution_PropertyChanged;
            sensorDataProvider.PropertyChanged += SensorDataProvider_PropertyChanged;

            if (!_debug)
                DigitalIOConnector.Instance.PropertyChanged += DigitalIOConnector_PropertyChanged;

            LastReadingPoints = new RangeAddableObservableCollection<Measurement>(_sensorDataProvider.LastReadingPoints);

            ChangePumpStatusCmd = ReactiveCommand.Create(DoChangePumpStatus);
        }

        public void StopReading()
        {
            _sensorDataProvider.PropertyChanged -= SensorDataProvider_PropertyChanged;
            _sensorDataProvider.Stop();

            _wateringExecution.PropertyChanged -= WateringExecution_PropertyChanged;
            _wateringExecution.Dispose();

            if (!_debug)
                DigitalIOConnector.Instance.PropertyChanged -= DigitalIOConnector_PropertyChanged;
        }

        private void DoChangePumpStatus()
        {
            if (DigitalIOConnector.Instance.PumpIsOn)
                DigitalIOConnector.Instance.TurnOffPump();
            else
                DigitalIOConnector.Instance.TurnOnPump();
        }

        private string _timeToMainWateringInfo;
        public string TimeToMainWateringInfo
        {
            get => _timeToMainWateringInfo;
            set => this.RaiseAndSetIfChanged(ref _timeToMainWateringInfo, value);
        }

        private string _timeToSecondaryWateringInfo;
        public string TimeToSecondaryWateringInfo
        {
            get => _timeToSecondaryWateringInfo;
            set => this.RaiseAndSetIfChanged(ref _timeToSecondaryWateringInfo, value);
        }


        private string _pumpButtonText = "Pumpen einschalten";
        public string PumpButtonText
        {
            get => _pumpButtonText;
            set => this.RaiseAndSetIfChanged(ref _pumpButtonText, value);
        }


        public bool DiagnosticModeActive
        {
            get => _sensorDataProvider.DiagnosticModeActive;
            set => _sensorDataProvider.DiagnosticModeActive = value;
        }


        private void DigitalIOConnector_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PumpButtonText = DigitalIOConnector.Instance.PumpIsOn ? "Pumpen ausschalten" : "Pumpen einschalten";
        }

        private void SensorDataProvider_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "LastReadingPoints") return;

            LastReadingPoints.Clear();
            LastReadingPoints.InsertRange(_sensorDataProvider.LastReadingPoints);
        }

        private void WateringExecution_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "TimeToWatering":
                    TimeToMainWateringInfo = _wateringExecution.TimeToWatering;
                    break;
                case "TimeToWatering2":
                    TimeToSecondaryWateringInfo = _wateringExecution.TimeToWatering2;
                    break;
            }
        }
    }
}
