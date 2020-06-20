using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Watering2.UserControls
{
    public class UcStatus : UserControl
    {
        public UcStatus()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
