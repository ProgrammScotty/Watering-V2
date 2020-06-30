using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Avalonia.Threading;
using DynamicData;
using Watering2.Models;
using Watering2.Services;
using Watering2.Utils;

namespace Watering2.ViewModels
{
    public class TabHistoryViewModel : ViewModelBase, IDisposable
    {
        private DataService _weatherDataService;
        private Action doUpdateReadingHistory;

        private List<Measurement> MeasurementList = new List<Measurement>()
        {
            new Measurement() { Id = 0, Pressure = 1000.2f, Humidity = 58.3f, Temperature = 20.2f, Raining = false, DewPoint = 10d, TimeStamp = DateTime.Now,},
            new Measurement() { Id = 1, Pressure = 990.5f, Humidity = 80.8f, Temperature = 17.2f, Raining = true, DewPoint = 11, TimeStamp = DateTime.Now.AddMinutes(10),}
        };

        public ObservableCollection<Measurement> WeatherDataList { get; private set; }

        //for the designer
        public TabHistoryViewModel()
        {
            WeatherDataList = new ObservableCollection<Measurement>(MeasurementList);

        }

        public TabHistoryViewModel(DataService service)
        {
            _weatherDataService = service;
            WeatherDataList = new ObservableCollection<Measurement>(MeasurementList);

            service.PropertyChanged += DataService_PropertyChanged;
            doUpdateReadingHistory = UpdateReadingHistory;
        }

        public void UpdateReadingHistory()
        {
            WeatherDataList.Clear();
            WeatherDataList.AddOrInsertRange(_weatherDataService.GetReadingPointsFirst(12*24), -1);
        }

        private void DataService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "readingPoint")
            {
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(doUpdateReadingHistory, DispatcherPriority.Background);
            }
        }


        public void Dispose()
        {
            if (_weatherDataService != null)
                _weatherDataService.PropertyChanged -= DataService_PropertyChanged;
        }
    }
}
