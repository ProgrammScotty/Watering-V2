﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Watering2.ViewModels
{
    public static class DesignViewModel
    {
        //for design time: Provide the same name of the view model is in run time
        public static MainWindowViewModel CfgViewModel { get; } = new MainWindowViewModel(new  TabStatusViewModel(), new TabConfigViewModel(null), new TabHistoryPumpViewModel(), new TabHistoryViewModel(), new TabGraphicViewModel(),null);
        public static MainWindowViewModel StatusViewModel { get; } = new MainWindowViewModel(new TabStatusViewModel(), new TabConfigViewModel(null), new TabHistoryPumpViewModel(), new TabHistoryViewModel(), new TabGraphicViewModel(),  null);
        public static MainWindowViewModel HistoryPumpViewModel { get; } = new MainWindowViewModel(new TabStatusViewModel(), new TabConfigViewModel(null), new TabHistoryPumpViewModel(), new TabHistoryViewModel(), new TabGraphicViewModel(),  null);
        public static MainWindowViewModel HistoryViewModel { get; } = new MainWindowViewModel(new TabStatusViewModel(), new TabConfigViewModel(null), new TabHistoryPumpViewModel(), new TabHistoryViewModel(), new TabGraphicViewModel(), null);
        public static MainWindowViewModel GraphicViewModel { get; } = new MainWindowViewModel(new TabStatusViewModel(), new TabConfigViewModel(null), new TabHistoryPumpViewModel(), new TabHistoryViewModel(), new TabGraphicViewModel(), null);
    }
}
