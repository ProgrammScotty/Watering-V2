using System;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;
using Watering2.Validators;

namespace Watering2.Models
{
    public class WateringConfig : ConfigurationSection, INotifyPropertyChanged
    {
        [ConfigurationProperty("MeasurementFrequency", DefaultValue = 600, IsRequired = true)]
        [IntegerValidator(MinValue = 60, MaxValue = 1200, ExcludeRange = false)]
        public int MeasurementFrequency { 
            get => (int)this["MeasurementFrequency"];
            set { this["MeasurementFrequency"] = value; OnPropertyChanged(nameof(MeasurementFrequency)); }
        }

        [ConfigurationProperty("PercentageHotFor2ndWateringActive", DefaultValue = 40.0d, IsRequired = true)]
        [DoubleValidator(minValue:15d,maxValue:90d)]
        public double PercentageHotFor2ndWateringActive
        {
            get => (double)this["PercentageHotFor2ndWateringActive"];
            set { this["PercentageHotFor2ndWateringActive"] = value; OnPropertyChanged(nameof(PercentageHotFor2ndWateringActive));}
        }

        [ConfigurationProperty("LevelHeatTemperature", DefaultValue = 35d, IsRequired = true)]
        [DoubleValidator(minValue:20d,maxValue:50d)]
        public double LevelHeatTemperature
        {
            get => (double)this["LevelHeatTemperature"];
            set { this["LevelHeatTemperature"] = value; OnPropertyChanged(nameof(LevelHeatTemperature));}
        }

        [ConfigurationProperty("LevelColdTemperature", DefaultValue = 15d, IsRequired = true)]
        [DoubleValidator(minValue: 0d, maxValue: 20d)]
        public double LevelColdTemperature
        {
            get => (double)this["LevelColdTemperature"];
            set { this["LevelColdTemperature"] = value; OnPropertyChanged(nameof(LevelColdTemperature));}
        }

        [ConfigurationProperty("PumpDurationMainCycle", DefaultValue = 90, IsRequired = true)]
        [IntegerValidator(MinValue = 0, MaxValue = 500, ExcludeRange = false)]
        public int PumpDurationMainCycle
        {
            get => (int)this["PumpDurationMainCycle"];
            set { this["PumpDurationMainCycle"] = value; OnPropertyChanged(nameof(PumpDurationMainCycle));}
        }

        [ConfigurationProperty("PumpDurationSecondCycle", DefaultValue = 60, IsRequired = true)]
        [IntegerValidator(MinValue = 0, MaxValue = 500, ExcludeRange = false)]
        public int PumpDurationSecondCycle
        {
            get => (int)this["PumpDurationSecondCycle"];
            set { this["PumpDurationSecondCycle"] = value; OnPropertyChanged(nameof(PumpDurationSecondCycle));}
        }

        [ConfigurationProperty("RainDurationToStopWatering", DefaultValue = "2:0:0", IsRequired = true)]
        public TimeSpan RainDurationToStopWatering
        {
            get => (TimeSpan)this["RainDurationToStopWatering"];
            set { this["RainDurationToStopWatering"] = value; OnPropertyChanged(nameof(RainDurationToStopWatering));}
        }

        [ConfigurationProperty("CorrFactorHeat", DefaultValue = 1.5d, IsRequired = true)]
        public double CorrFactorHeat
        {
            get => (double)this["CorrFactorHeat"];
            set {this["CorrFactorHeat"] = value; OnPropertyChanged(nameof(CorrFactorHeat)); }
        }

        [ConfigurationProperty("CorrFactorCold", DefaultValue = 0.5d, IsRequired = true)]
        [DoubleValidator(minValue: 0.1d, maxValue: 0.99d)]
        public double CorrFactorCold
        {
            get => (double)this["CorrFactorCold"];
            set { this["CorrFactorCold"] = value; OnPropertyChanged(nameof(CorrFactorCold));}
        }

        [ConfigurationProperty("MaxDaysWithoutWatering", DefaultValue = 3, IsRequired = true)]
        [IntegerValidator(MinValue = 0, MaxValue = 5, ExcludeRange = false)]
        public int MaxDaysWithoutWatering
        {
            get => (int)this["MaxDaysWithoutWatering"];
            set { this["MaxDaysWithoutWatering"] = value; OnPropertyChanged(nameof(MaxDaysWithoutWatering));}
        }

        [ConfigurationProperty("SecondWateringActive", DefaultValue = true, IsRequired = true)]
        public bool SecondWateringActive
        {
            get => (bool)this["SecondWateringActive"];
            set { this["SecondWateringActive"] = value; OnPropertyChanged(nameof(SecondWateringActive));}
        }

        [ConfigurationProperty("StartWateringMainCycle", DefaultValue = "6:0:0", IsRequired = true)]
        public TimeSpan StartWateringMainCycle
        {
            get => (TimeSpan)this["StartWateringMainCycle"];
            set { this["StartWateringMainCycle"] = value; OnPropertyChanged(nameof(StartWateringMainCycle));}
        }

        [ConfigurationProperty("StartWateringSecondCycle", DefaultValue = "18:0:0", IsRequired = true)]
        public TimeSpan StartWateringSecondCycle
        {
            get => (TimeSpan)this["StartWateringSecondCycle"];
            set { this["StartWateringSecondCycle"] = value; OnPropertyChanged(nameof(StartWateringSecondCycle));}
        }

        [ConfigurationProperty("BeginMonitoring", DefaultValue = "8:0:0", IsRequired = true)]
        public TimeSpan BeginMonitoring
        {
            get => (TimeSpan)this["BeginMonitoring"];
            set { this["BeginMonitoring"] = value; OnPropertyChanged(nameof(BeginMonitoring));}
        }

        [ConfigurationProperty("BeginAfternoonMonitoring", DefaultValue = "13:0:0", IsRequired = true)]
        public TimeSpan BeginAfternoonMonitoring
        {
            get => (TimeSpan)this["BeginAfternoonMonitoring"];
            set { this["BeginAfternoonMonitoring"] = value; OnPropertyChanged(nameof(BeginAfternoonMonitoring)); }
        }

        [ConfigurationProperty("EndMonitoring", DefaultValue = "19:0:0", IsRequired = true)]
        public TimeSpan EndMonitoring
        {
            get => (TimeSpan)this["EndMonitoring"];
            set { this["EndMonitoring"] = value; OnPropertyChanged(nameof(EndMonitoring));}
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
