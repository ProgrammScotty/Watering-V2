using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Avalonia.Data.Converters;

namespace Watering2.ValueConverters
{
    public class SecondToMinSecValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                double seconds = (double)value;
                TimeSpan ts = TimeSpan.FromSeconds(seconds);
                return $"{ts.Minutes}m {ts.Seconds}s";
            }
            catch (Exception)
            {
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
