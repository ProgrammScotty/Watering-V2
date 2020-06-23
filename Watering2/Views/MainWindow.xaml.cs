using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Watering2.Utils;
using Watering2.ViewModels;

namespace Watering2.Views
{
    public class MainWindow : Window
    {
        private MainWindowViewModel _viewModelMainWnd;
        private TabControl _mainTabCtrl;
        private TabItem _historyTabItem;
        private TabItem _pumpHistoryTabItem;
        private TabItem _statisticsTabItem;

        public MainWindow()
        {
            this.Opened += MainWindow_Opened;
            this.DataContextChanged += MainWindow_DataContextChanged;

            InitializeComponent();
#if DEBUG
            //this.AttachDevTools();
#endif

        }

        private void MainWindow_DataContextChanged(object sender, System.EventArgs e)
        {
            _viewModelMainWnd = this.DataContext as MainWindowViewModel;
            if (_viewModelMainWnd == null)
                return;

            _mainTabCtrl = this.FindControl<TabControl>("TabCtrl");
            if (_mainTabCtrl == null)
                return;

            _historyTabItem = this.FindControl<TabItem>("TabHistoryReadings");
            _pumpHistoryTabItem = this.FindControl<TabItem>("TabHistoryPump");
            _statisticsTabItem = this.FindControl<TabItem>("TabStatistics");

            _mainTabCtrl.SelectionChanged += MainTabCtrl_SelectionChanged;
        }

        private void MainTabCtrl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (_historyTabItem != null && _historyTabItem.IsSelected)
            //    _viewModelMainWnd.HistoryViewModel.UpdateReadingHistory();
            if (_pumpHistoryTabItem != null && _pumpHistoryTabItem.IsSelected)
                _viewModelMainWnd.HistoryPumpViewModel.UpdatePumpHistory();
            //else if (_statisticsTabItem != null && _statisticsTabItem.IsSelected)
            //    _viewModelMainWnd.StatisticsViewModel.UpdateReadings();
        }

        private void MainWindow_Opened(object sender, System.EventArgs e)
        {
            if (DataContext is ICloseable)
            {
                ((ICloseable)DataContext).RequestClose += MainWindow_RequestClose;
            }
        }

        private void MainWindow_RequestClose(object sender, System.EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch
            {

            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
