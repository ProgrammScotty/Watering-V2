using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.I2c;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using Watering2.Configuration;
using Bmxx80;
using UnitsNet;
using Watering2.Models;

namespace Watering2.Services
{
    public class SensorConnector : IDisposable, INotifyPropertyChanged
    {
        private const int busId = 1;
        private Pressure _defaultSeaLevelPreassure = WeatherHelper.MeanSeaLevel;
        private Bme280 _bme280;

        private ConfigController _cfgCtrl;
        private bool _debugMode;
        private int _readingInterval;
        private Measurement _lastReadingPoint;
        private Timer _mainTimer;
        private bool _diagnosticMode;
        private int _diagnosticReadingInterval = 6000;
        private Random _random = new Random(1);


        public Measurement GetLastReadingPoint() => _lastReadingPoint;
        public bool IsInDiagnosticMode => _diagnosticMode;
        
        public SensorConnector(ConfigController cfgController, bool debugMode)
        {
            _cfgCtrl = cfgController;
            _debugMode = debugMode;

            if(!debugMode) CreateSensor();

            if (!_debugMode)
            {
                _readingInterval = _cfgCtrl.Configuration.MeasurementFrequency;
            }
            else
            {
                _readingInterval = 60; //sec für memory leak
                //_readingInterval = 60*60; //sec für data binding statistics
            }

            _readingInterval *= 1000;

            _lastReadingPoint = new Measurement();
            _mainTimer = new Timer(ReadSensor, null, Timeout.Infinite, Timeout.Infinite);

            _cfgCtrl.PropertyChanged += CfgCtrl_PropertyChanged;
        }

        public void StartDiagnosticMode()
        {
            if (_diagnosticMode)
                return;

            _diagnosticMode = true;

            _mainTimer.Change(Timeout.Infinite, Timeout.Infinite);
            ReadSensor(null);
        }

        public void StopDiagnosticMode()
        {
            if (!_diagnosticMode)
                return;

            _diagnosticMode = false;

            _mainTimer.Change(Timeout.Infinite, Timeout.Infinite);
            ReadSensor(null);
        }

        public bool StartReading()
        {
            _mainTimer.Change(0, _cfgCtrl.Configuration.PumpDurationMainCycle*1000);
            return true;
        }

        public bool StopReading()
        {
            _mainTimer.Change(Timeout.Infinite, Timeout.Infinite);
            return true;
        }


        private void CreateSensor()
        {
            var i2cSettings = new I2cConnectionSettings(busId, Bmx280Base.SecondaryI2cAddress);
            var i2cDevice = I2cDevice.Create(i2cSettings);
            _bme280 = new Bme280(i2cDevice);

            // set higher sampling
            _bme280.TemperatureSampling = Sampling.LowPower;
            _bme280.PressureSampling = Sampling.UltraHighResolution;
            _bme280.HumiditySampling = Sampling.Standard;

            // set mode forced so device sleeps after read
            _bme280.SetPowerMode(Bmx280PowerMode.Forced);
        }


        private void ReadSensor(object state)
        {
            if (!_debugMode && _bme280 == null)
            {
                return;
            }

            if (!_debugMode)
            {
                // set mode forced and read again
                _bme280.SetPowerMode(Bmx280PowerMode.Forced);

                // wait for measurement to be performed
                var measurementTime = _bme280.GetMeasurementDuration();
                Thread.Sleep(measurementTime);

                // read values
                _bme280.TryReadTemperature(out var tempValue);
                _bme280.TryReadPressure(out var preValue);
                _bme280.TryReadHumidity(out var humValue);

                _lastReadingPoint = new Measurement()
                {
                    TimeStamp = DateTime.Now,
                    Temperature = tempValue.DegreesCelsius,
                    Pressure = preValue.Hectopascals,
                    Humidity = humValue.Percent,
                    DewPoint = WeatherHelper.CalculateDewPoint(tempValue, humValue).DegreesCelsius,
                    Raining = DigitalIOConnector.Instance.ReadRainSensorDetectsRain()
                };
            }
            else
            {
                _lastReadingPoint = new Measurement()
                {
                    TimeStamp = DateTime.Now,
                    Temperature = (double)_random.Next(1000,4000)/100d,
                    Pressure = (double)_random.Next(95000, 110000) / 100d,
                    Humidity = (double)_random.Next(5000, 9000) / 100d,
                    DewPoint = 12.12345d,
                    Raining = _random.Next(0,1) == 1
                };
            }

            OnPropertyChanged("Reading done");
        }

        private void CfgCtrl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            int newReadingInterval = _cfgCtrl.Configuration.MeasurementFrequency;
            newReadingInterval *= 1000;
            if (newReadingInterval == _readingInterval)
                return;

            _readingInterval = newReadingInterval;
            _mainTimer.Change(Timeout.Infinite, Timeout.Infinite);
            ReadSensor(null);
        }

        public void Dispose()
        {
            _bme280?.Dispose();
            DigitalIOConnector.Instance.Dispose();
            _mainTimer?.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
