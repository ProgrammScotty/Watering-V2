using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Avalonia.Data.Converters;

namespace Watering2.ValueConverters
{
    public class TimeSpanValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan time)
            {
                return $"{time.Hours:00}:{time.Minutes:00}:{time.Seconds:00}";
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string time)
            {
                TimeSpan.TryParse(time, out var timeSpan);
                return timeSpan;
            }

            return new TimeSpan(-1, 0, 0, 0);
        }
    }
}
