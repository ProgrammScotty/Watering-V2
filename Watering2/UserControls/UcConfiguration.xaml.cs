using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Watering2.UserControls
{
    public class UcConfiguration : UserControl
    {
        public UcConfiguration()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
