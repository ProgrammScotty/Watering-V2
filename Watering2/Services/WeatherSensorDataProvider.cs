using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;
using Watering2.Configuration;
using Watering2.Models;

namespace Watering2.Services
{
    public class WeatherSensorDataProvider : INotifyPropertyChanged
    {
        private static object _lockObject = new object();
        private const int MaxNumLastReadingPoints = 3;
        private static List<Measurement> _lastReadingPoints = new List<Measurement>(MaxNumLastReadingPoints);

        private SensorConnector _sensorConnector;
        private DataService _dataService;
        private ConfigController _cfgController;


        public List<Measurement> LastReadingPoints
        {
            get
            {
                lock (_lockObject)
                {
                    //should be the same list object? DataBinding?
                    return _lastReadingPoints;
                }
            }
        }

        public WeatherSensorDataProvider(DataService dataService, ConfigController cfgController, bool debug)
        {
            _lastReadingPoints.Add(new Measurement() { Humidity = 0, Pressure = 0, Temperature = 0, DewPoint = 0, Raining = false, TimeStamp = DateTime.Now });

            _dataService = dataService;
            _cfgController = cfgController;

            _sensorConnector = new SensorConnector(_cfgController, debug);
            _sensorConnector.PropertyChanged += SensorConnector_PropertyChanged;
            _sensorConnector.StartReading();
        }

        public bool DiagnosticModeActive
        {
            get => _sensorConnector?.IsInDiagnosticMode ?? false;
            set
            {
                if (_sensorConnector == null)
                    return;
                if (value)
                    _sensorConnector.StartDiagnosticMode();
                else
                    _sensorConnector.StopDiagnosticMode();
            }
        }

        public void Stop()
        {
            lock (_lockObject)
            {
                if (_sensorConnector != null)
                {
                    _sensorConnector.StopReading();
                    _sensorConnector.Dispose();
                }
            }
        }

        private void SensorConnector_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Reading done") return;

            var readingPoint = _sensorConnector.GetLastReadingPoint();

            lock (_lockObject)
            {
                if (_lastReadingPoints.Count >= MaxNumLastReadingPoints)
                    _lastReadingPoints.RemoveAt(MaxNumLastReadingPoints - 1);
                _lastReadingPoints.Insert(0, readingPoint);
            }

            if (_sensorConnector != null && !_sensorConnector.IsInDiagnosticMode)
                _dataService.SaveReadingPoint(readingPoint);

            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("LastReadingPoints"));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
