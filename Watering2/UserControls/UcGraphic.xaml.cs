using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Watering2.UserControls
{
    public class UcGraphic : UserControl
    {
        public UcGraphic()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
