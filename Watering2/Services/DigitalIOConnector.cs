using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Device.Gpio;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace Watering2.Services
{
    public class DigitalIOConnector : INotifyPropertyChanged, IDisposable
    {
        private static DigitalIOConnector _instance = null;
        private const int PinPump = 17;
        private const int PinRainensor = 18;
        private GpioController _gpioCtrl;

        public static DigitalIOConnector Instance => _instance ??= new DigitalIOConnector();
        public bool OutputIsHigh = false;

        private DigitalIOConnector()
        {
            _gpioCtrl = new GpioController();
            _gpioCtrl.OpenPin(PinPump, PinMode.Output);
            _gpioCtrl.OpenPin(PinRainensor, PinMode.InputPullDown);
            _gpioCtrl.Write(PinPump,PinValue.Low);
        }

        public bool PumpIsOn
        {
            get => OutputIsHigh;
            set { OutputIsHigh = value; OnPropertyChanged(nameof(PumpIsOn)); }
        }

        public void TurnOnPump()
        {
            _gpioCtrl.Write(PinPump, PinValue.High);
            OutputIsHigh = true;
            OnPropertyChanged(nameof(OutputIsHigh));
        }

        public void TurnOffPump()
        {
            _gpioCtrl.Write(PinPump, PinValue.Low);
            OutputIsHigh = false;
            OnPropertyChanged(nameof(OutputIsHigh));
        }

        public bool ReadRainSensorDetectsRain()
        {
            return _gpioCtrl.Read(PinRainensor) == PinValue.Low;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            if (_gpioCtrl != null)
            {
                if(_gpioCtrl.IsPinOpen(PinPump))
                    _gpioCtrl.Write(PinPump, PinValue.Low);

                _gpioCtrl.Dispose();
                _gpioCtrl = null;
            }
        }
    }
}
