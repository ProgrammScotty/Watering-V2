using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Watering2.UserControls
{
    public class UcHistoryPump : UserControl
    {
        public UcHistoryPump()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
