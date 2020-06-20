using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using ReactiveUI;
using ReactiveUI.Validation;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Watering2.Configuration;
using Watering2.Models;


namespace Watering2.ViewModels
{
    public class TabConfigViewModel : ViewModelBase, IValidatableViewModel
    {
        private ConfigController _cfgController;
        private int _pumpDurationMainCycle;
        private int _pumpDurationSecondCycle;
        private TimeSpan _startWateringMainCycle;
        private TimeSpan _startWateringSecondCycle;
        private double _levelHeatTemperature;
        private double _levelColdTemperature;
        private double _corrFactorHeat;
        private double _corrFactorCold;
        private int _measurementFrequency;
        private TimeSpan _beginMonitoring;
        private TimeSpan _endMonitoring;
        private TimeSpan _rainDurationToStopWatering;
        private double _percentageHotFor2ndWateringActive;
        private bool _secondWateringActive;
        private int _maxDaysWithoutWatering;
        private string _validationErrorText;


        public ReactiveCommand<Unit, Unit> SaveConfigurationCmd { get; }
        public ReactiveCommand<Unit, Unit> RestoreConfigurationCmd { get; }

        public ValidationContext ValidationContext { get; } = new ValidationContext();
        public ValidationHelper MeasurementIntervalRule { get; }
        public ValidationHelper WateringTimeRule { get; }
        public ValidationHelper MeasureStartTimeRule { get; }
        public ValidationHelper MeasureEndTimeRule { get; }
        public ValidationHelper MeasureIntervalRule { get; }
        public ValidationHelper LevelHotTempRule { get; }
        public ValidationHelper CorrectionHotRule { get; }
        public ValidationHelper LevelColdTempRule { get; }
        public ValidationHelper CorrectionColdRule { get; }
        public ValidationHelper WateringDurationRule { get; }
        public ValidationHelper MaxDaysWithoutWateringRule { get; }

        public TabConfigViewModel()
        {

        }

        public TabConfigViewModel(ConfigController cfgController)
        {
            if(cfgController == null)
                return;

            _cfgController = cfgController;
            ReadConfigValues();
            
            SaveConfigurationCmd = ReactiveCommand.Create(DoSaveConfiguration);
            RestoreConfigurationCmd = ReactiveCommand.Create(DoRestoreConfiguration);

            MeasurementIntervalRule = this.ValidationRule(
                viewModel => viewModel.MeasurementFrequency,
                intervall => intervall >= 60 && intervall <= 3600,
                intervall => $"Ungültiger Wert: Messzyklus (600-3600), Eingabe: {intervall}");

            WateringTimeRule = this.ValidationRule(
                viewmodel => viewmodel.StartWateringMainCycle,
                time => time.Days == 0 && time.Hours >= 0 && time.Minutes >= 0 && time.Seconds >= 0 && time.Days == 0,
                time => $"Ungültiger Zeitpunkt für das Gießen ({time})");

            MeasureStartTimeRule = this.ValidationRule(
                viewmodel => viewmodel.BeginMonitoring,
                time => time.Days == 0 && time.Hours >= 0 && time.Minutes >= 0 && time.Seconds >= 0 && time.Days == 0,
                time => $"Ungültiger Zeitpunkt für den Beginn der Auswertung ({time})");

            MeasureEndTimeRule = this.ValidationRule(
                viewmodel => viewmodel.EndMonitoring,
                time => time.Days == 0 && time.Hours >= 0 && time.Minutes >= 0 && time.Seconds >= 0 && time.Days == 0,
                time => $"Ungültiger Zeitpunkt für das Ende der Auswertung ({time})");

            var startAndEndTimeValid = this
                .WhenAnyValue(x => x.BeginMonitoring, x => x.EndMonitoring, (begin, end) => new { Begin = begin, End = end })
                .Select(x => x.End - x.Begin > new TimeSpan(0, 0, 0));

            MeasureIntervalRule = this.ValidationRule(
                _ => startAndEndTimeValid,
                (vm, state) => !state ? "Start time later then end time" : string.Empty);

            LevelHotTempRule = this.ValidationRule(
                viewmodel => viewmodel.LevelHeatTemperature,
                levelHot => levelHot > 20,
                "Ungültiger Wert: Schwellwert Hitze (>20°");

            CorrectionHotRule = this.ValidationRule(
                viewmodel => viewmodel.CorrFactorHeat,
                corrHot => corrHot >= 1,
                "Ungültiger Wert: Korrektur Hitze (>1)");

            LevelColdTempRule = this.ValidationRule(
                viewmodel => viewmodel.LevelColdTemperature,
                levelCold => levelCold <= 15 && levelCold > 5,
                "Ungültiger Wert: Schwellwert Kälte (5>L<=15");

            CorrectionColdRule = this.ValidationRule(
                viewmodel => viewmodel.CorrFactorCold,
                corrCold => corrCold < 1 && corrCold > 0,
                "Ungültiger Wert: Korrekturwert Kälte (<1)");

            WateringDurationRule = this.ValidationRule(
                viewmodel => viewmodel.PumpDurationMainCycle,
                duration => duration > 60,
                "Ungültiger Wert: Gießzeit (>60)");

            MaxDaysWithoutWateringRule = this.ValidationRule(
                viewModel => viewModel.MaxDaysWithoutWatering,
                days => days >= 0,
                days => $"Ungültiger Wert: Max Anzahl Tage ohne Gießen: >=0: {days}");


            var canSave = this.IsValid();

            SaveConfigurationCmd = ReactiveCommand.CreateFromTask(async unit => { DoSaveConfiguration(); }, canSave);

            ValidationContext.Valid.Subscribe(valid =>
            {
                ValidationErrorText = valid ? string.Empty : this.ValidationContext.Text.ToSingleLine();
            });
        }

        public string ValidationErrorText
        {
            get => this.ValidationContext.Text.ToSingleLine();
            set => this.RaiseAndSetIfChanged(ref this._validationErrorText, value);
        }


        private void ReadConfigValues()
        {
            _pumpDurationMainCycle = _cfgController.Configuration.PumpDurationMainCycle;
            _pumpDurationSecondCycle = _cfgController.Configuration.PumpDurationSecondCycle;
            _startWateringMainCycle = _cfgController.Configuration.StartWateringMainCycle;
            _startWateringSecondCycle = _cfgController.Configuration.StartWateringSecondCycle;
            _levelHeatTemperature = _cfgController.Configuration.LevelHeatTemperature;
            _levelColdTemperature = _cfgController.Configuration.LevelColdTemperature;
            _corrFactorHeat = _cfgController.Configuration.CorrFactorHeat;
            _corrFactorCold = _cfgController.Configuration.CorrFactorCold;
            _measurementFrequency = _cfgController.Configuration.MeasurementFrequency;
            _beginMonitoring = _cfgController.Configuration.BeginMonitoring;
            _endMonitoring = _cfgController.Configuration.EndMonitoring;
            _rainDurationToStopWatering = _cfgController.Configuration.RainDurationToStopWatering;
            _percentageHotFor2ndWateringActive = _cfgController.Configuration.PercentageHotFor2ndWateringActive;
            _secondWateringActive = _cfgController.Configuration.SecondWateringActive;
            _maxDaysWithoutWatering = _cfgController.Configuration.MaxDaysWithoutWatering;
        }

        private void DoSaveConfiguration()
        {
            _cfgController.Configuration.PumpDurationMainCycle = _pumpDurationMainCycle;
            _cfgController.Configuration.PumpDurationSecondCycle = _pumpDurationSecondCycle;
            _cfgController.Configuration.StartWateringMainCycle = _startWateringMainCycle;
            _cfgController.Configuration.StartWateringSecondCycle = _startWateringSecondCycle;
            _cfgController.Configuration.LevelHeatTemperature = _levelHeatTemperature;
            _cfgController.Configuration.LevelColdTemperature = _levelColdTemperature;
            _cfgController.Configuration.CorrFactorHeat = _corrFactorHeat;
            _cfgController.Configuration.CorrFactorCold = _corrFactorCold;
            _cfgController.Configuration.MeasurementFrequency = _measurementFrequency;
            _cfgController.Configuration.BeginMonitoring = _beginMonitoring;
            _cfgController.Configuration.EndMonitoring = _endMonitoring;
            _cfgController.Configuration.RainDurationToStopWatering = _rainDurationToStopWatering;
            _cfgController.Configuration.PercentageHotFor2ndWateringActive = _percentageHotFor2ndWateringActive;
            _cfgController.Configuration.SecondWateringActive = _secondWateringActive;
            _cfgController.Configuration.MaxDaysWithoutWatering = _maxDaysWithoutWatering;
            _cfgController.SaveSettings();
        }

        private void DoRestoreConfiguration()
        {
            PumpDurationMainCycle = _cfgController.Configuration.PumpDurationMainCycle;
            PumpDurationSecondCycle = _cfgController.Configuration.PumpDurationSecondCycle;
            StartWateringMainCycle = _cfgController.Configuration.StartWateringMainCycle;
            StartWateringSecondCycle = _cfgController.Configuration.StartWateringSecondCycle;
            LevelHeatTemperature = _cfgController.Configuration.LevelHeatTemperature;
            LevelColdTemperature = _cfgController.Configuration.LevelColdTemperature;
            CorrFactorHeat = _cfgController.Configuration.CorrFactorHeat;
            CorrFactorCold = _cfgController.Configuration.CorrFactorCold;
            MeasurementFrequency = _cfgController.Configuration.MeasurementFrequency;
            BeginMonitoring = _cfgController.Configuration.BeginMonitoring;
            EndMonitoring = _cfgController.Configuration.EndMonitoring;
            RainDurationToStopWatering = _cfgController.Configuration.RainDurationToStopWatering;
            PercentageHotFor2ndWateringActive = _cfgController.Configuration.PercentageHotFor2ndWateringActive;
            SecondWateringActive = _cfgController.Configuration.SecondWateringActive;
            MaxDaysWithoutWatering = _cfgController.Configuration.MaxDaysWithoutWatering;
        }

        public int MaxDaysWithoutWatering
        {
            get => _maxDaysWithoutWatering;
            set => this.RaiseAndSetIfChanged(ref _maxDaysWithoutWatering, value);
        }

        public bool SecondWateringActive
        {
            get => _secondWateringActive;
            set => this.RaiseAndSetIfChanged(ref _secondWateringActive, value);
        }

        public double PercentageHotFor2ndWateringActive
        {
            get => _percentageHotFor2ndWateringActive;
            set => this.RaiseAndSetIfChanged(ref _percentageHotFor2ndWateringActive, value);
        }

        public TimeSpan RainDurationToStopWatering
        {
            get => _rainDurationToStopWatering;
            set => this.RaiseAndSetIfChanged(ref _rainDurationToStopWatering, value);
        }

        public TimeSpan EndMonitoring
        {
            get => _endMonitoring;
            set => this.RaiseAndSetIfChanged(ref _endMonitoring, value);
        }

        public TimeSpan BeginMonitoring
        {
            get => _beginMonitoring;
            set => this.RaiseAndSetIfChanged(ref _beginMonitoring, value);
        }

        public int MeasurementFrequency
        {
            get => _measurementFrequency;
            set => this.RaiseAndSetIfChanged(ref _measurementFrequency, value);
        }

        public double CorrFactorCold
        {
            get => _corrFactorCold;
            set => this.RaiseAndSetIfChanged(ref _corrFactorCold, value);
        }

        public double CorrFactorHeat
        {
            get => _corrFactorHeat;
            set => this.RaiseAndSetIfChanged(ref _corrFactorHeat, value);
        }

        public double LevelColdTemperature
        {
            get => _levelColdTemperature;
            set => this.RaiseAndSetIfChanged(ref _levelColdTemperature, value);
        }

        public double LevelHeatTemperature
        {
            get => _levelHeatTemperature;
            set => this.RaiseAndSetIfChanged(ref _levelHeatTemperature, value);
        }

        public TimeSpan StartWateringSecondCycle
        {
            get => _startWateringSecondCycle;
            set => this.RaiseAndSetIfChanged(ref _startWateringSecondCycle, value);
        }

        public TimeSpan StartWateringMainCycle
        {
            get => _startWateringMainCycle;
            set => this.RaiseAndSetIfChanged(ref _startWateringMainCycle, value);
        }

        public int PumpDurationSecondCycle
        {
            get => _pumpDurationSecondCycle;
            set => this.RaiseAndSetIfChanged(ref _pumpDurationSecondCycle, value);
        }

        public int PumpDurationMainCycle
        {
            get => _pumpDurationMainCycle;
            set => this.RaiseAndSetIfChanged(ref _pumpDurationMainCycle, value);
        }
    }
}
