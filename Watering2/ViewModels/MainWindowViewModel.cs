using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reactive;
using System.Text;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;
using Watering2.Utils;

namespace Watering2.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, ICloseable
    {
        public string Greeting => "Welcome to Avalonia!";
        private Window _topWindow;

       public ReactiveCommand<Window, Unit> CloseWindowCmd { get; }

        public TabConfigViewModel ConfigViewModel { get; private set; }
        public TabStatusViewModel StatusViewModel { get; private set; }
        public TabHistoryPumpViewModel HistoryPumpViewModel { get; private set; }
        public TabHistoryViewModel HistoryViewModel { get; private set; }
        public TabGraphicViewModel GraphicViewModel { get; private set; }

        public MainWindowViewModel()
        {
            //ConfigViewModel = null;
            //HistoryPumpViewModel = null;
        }

        public MainWindowViewModel(TabStatusViewModel tabStatusViewModel, TabConfigViewModel tabConfigViewModel, TabHistoryPumpViewModel tabHistoryPumpViewModel, TabHistoryViewModel tabHistoryViewModel, TabGraphicViewModel tabGraphicViewModel,  Window topWindow)
        {
            ConfigViewModel = tabConfigViewModel;
            StatusViewModel = tabStatusViewModel;
            HistoryPumpViewModel = tabHistoryPumpViewModel;
            HistoryViewModel = tabHistoryViewModel;
            GraphicViewModel = tabGraphicViewModel;

            _topWindow = topWindow;

            //CloseWindowCmd = ReactiveCommand.Create(new Action<Window>(CloseMainWindow));
           CloseWindowCmd = ReactiveCommand.Create<Window>(new Action<Window>(CloseWindow));
        }


        private void CloseWindow(Window window)
        {
            RequestClose?.Invoke(this,null);
        }

        private void CloseMainWindow()
        {
            Dispatcher.UIThread.InvokeAsync(() => _topWindow.Close());
            //_topWindow.Close(null);
        }

        public event EventHandler<EventArgs> RequestClose;
    }
}
