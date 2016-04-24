using System;
using System.Globalization;
using System.Windows.Data;

namespace OptionPricing.Converters {
    public class TimeSpanDays : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return ((TimeSpan) value).Days;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}