using System;
using System.Collections.Generic;
using System.Text;

namespace Watering2.ViewModels
{
    public static class DesignViewModel
    {
        //for design time: Provide the same name of the view model is in run time
        public static MainWindowViewModel CfgViewModel { get; } = new MainWindowViewModel(new  TabStatusViewModel(), new TabConfigViewModel(null), null);
        public static MainWindowViewModel StatusViewModel { get; } = new MainWindowViewModel(new TabStatusViewModel(), new TabConfigViewModel(null), null);
    }
}
