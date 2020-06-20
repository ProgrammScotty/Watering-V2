using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Avalonia.Data.Converters;

namespace Watering2.ValueConverters
{
    public class NumericValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string formatStrg = parameter as string;
                double valueDouble = (double)value;
                //if (string.IsNullOrEmpty(formatStrg))
                //    return valueDouble.ToString("##.##");

                var doof = string.IsNullOrEmpty(formatStrg) ? $"{valueDouble:#.##}" : string.Format(formatStrg, valueDouble);
                return doof;
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
