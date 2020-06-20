using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Watering2.Utils;

namespace Watering2.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            this.Opened += MainWindow_Opened;

            InitializeComponent();
#if DEBUG
            //this.AttachDevTools();
#endif

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
