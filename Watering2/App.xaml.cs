using System;

using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Watering2.Configuration;
using Watering2.Models;
using Watering2.Services;
using Watering2.ViewModels;
using Watering2.Views;

namespace Watering2
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            TabConfigViewModel tabConfigViewModel = null;
            TabStatusViewModel statusViewModel = null;
            TabHistoryPumpViewModel historyPumpViewModel = null;
            TabHistoryViewModel historyViewModel = null;
            //TabHistoryPumpViewModel pumpHistoryViewModel = null;
            DataService wateringDataService = null;
            WateringExecution wateringExecution;
            WeatherSensorDataProvider sensorDataProvider;

            bool debug = false;
            #if DEBUG
            debug = true;
            #endif

            if (!Design.IsDesignMode)
            {
                Log.Logger = new LoggerConfiguration()
                    .Enrich.WithThreadId()
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails()
                    .MinimumLevel.Debug()
                    .WriteTo.File("Watering.log",
                    rollingInterval: RollingInterval.Day,
                    shared: true,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}]<{ThreadId}>{SourceContext} {Message} {Properties} {NewLine}{ExceptionDetails}")
                    .WriteTo.Console(LogEventLevel.Information, "{Timestamp:yyyy-MM-dd HH:mm:ss} {Message}")
                    .CreateLogger();
                
                Log.Information("Program starting...");
                ConfigController cfgController = new ConfigController();
                tabConfigViewModel = new TabConfigViewModel(cfgController);
                wateringDataService = new DataService();
                wateringExecution = new WateringExecution(cfgController, wateringDataService, debug);
                sensorDataProvider = new WeatherSensorDataProvider(wateringDataService, cfgController, debug);
                statusViewModel = new TabStatusViewModel(cfgController, sensorDataProvider, wateringExecution, debug);
                historyPumpViewModel = new TabHistoryPumpViewModel(wateringDataService);
                historyViewModel = new TabHistoryViewModel(wateringDataService);
            }


            IClassicDesktopStyleApplicationLifetime desktop = null;
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
            {
                
                desktop = lifetime;
                desktop.MainWindow = new MainWindow();
                desktop.MainWindow.DataContext = tabConfigViewModel == null ? new MainWindowViewModel() 
                    : new MainWindowViewModel(statusViewModel, tabConfigViewModel, historyPumpViewModel, historyViewModel, desktop.MainWindow);
            }

            

            if (desktop != null) desktop.Exit += Desktop_Exit;

            base.OnFrameworkInitializationCompleted();
        }

        private void Desktop_Exit(object sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            Log.CloseAndFlush();
        }
    }
}
