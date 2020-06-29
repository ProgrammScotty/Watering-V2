using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Watering2.UserControls
{
    public class UcWeatherHistory : UserControl
    {
        public UcWeatherHistory()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
