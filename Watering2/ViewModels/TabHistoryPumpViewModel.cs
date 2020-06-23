using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Threading;
using Watering2.Models;
using Watering2.Services;
using Watering2.Utils;

namespace Watering2.ViewModels
{
    public class TabHistoryPumpViewModel : ViewModelBase, IDisposable
    {
        private DataService _weatherDataService;
        private Action doUpdatePumpHistory;

        public RangeAddableObservableCollection<WateringData> DataList { get; private set; }

        //for the designer
        public TabHistoryPumpViewModel()
        {
            MakeTestData();
        }

        public TabHistoryPumpViewModel(DataService dbDataServiceService)
        {
            _weatherDataService = dbDataServiceService;
            MakeTestData();
            _weatherDataService.PropertyChanged += WeatherDataService_PropertyChanged;
            doUpdatePumpHistory = UpdatePumpHistory;
        }


        public void UpdatePumpHistory()
        {
            DataList.Clear();
            DataList.InsertRange(_weatherDataService.GetTracePointsFirst(200));
        }
        private void MakeTestData()
        {
            WateringData data1 = new WateringData()
            {
                CorrHot = 1,
                CorrCold = 1,
                SamplesCount = 200,
                SamplesHot = 50,
                SamplesCold = 0,
                DurationRain = 0,
                Duration = 90,
                TimeStamp = DateTime.Now
            };
            WateringData data2 = new WateringData()
            {
                CorrHot = 1,
                CorrCold = 1,
                SamplesCount = 200,
                SamplesHot = 40,
                SamplesCold = 0,
                DurationRain = 0,
                Duration = 60,
                TimeStamp = DateTime.Now.AddMinutes(15)
            };
            WateringData data3 = new WateringData()
            {
                CorrHot = 1,
                CorrCold = 1,
                SamplesCount = 200,
                SamplesHot = 150,
                SamplesCold = 0,
                DurationRain = 0,
                Duration = 60,
                TimeStamp = DateTime.Now.AddMinutes(30)
            };
            WateringData data4 = new WateringData()
            {
                CorrHot = 1,
                CorrCold = 1,
                SamplesCount = 200,
                SamplesHot = 150,
                SamplesCold = 0,
                DurationRain = 0,
                Duration = 60,
                TimeStamp = DateTime.Now.AddMinutes(45)
            };
            WateringData data5 = new WateringData()
            {
                CorrHot = 1,
                CorrCold = 1,
                SamplesCount = 200,
                SamplesHot = 150,
                SamplesCold = 0,
                DurationRain = 0,
                Duration = 60,
                TimeStamp = DateTime.Now.AddMinutes(60)
            };

            DataList = new RangeAddableObservableCollection<WateringData>
            {
                data1,
                data2,
                data3,
                data4,
                data5
            };
        }

        private void WeatherDataService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SaveWateringData")
            {
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(doUpdatePumpHistory, DispatcherPriority.Background);
            }
        }

        public void Dispose()
        {
            if (_weatherDataService != null)
                _weatherDataService.PropertyChanged -= WeatherDataService_PropertyChanged;
        }
    }
}
