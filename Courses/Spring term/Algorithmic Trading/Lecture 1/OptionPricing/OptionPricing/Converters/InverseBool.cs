// (c) Konstantin Brownstein 2016

using System;
using System.Globalization;
using System.Windows.Data;

namespace OptionPricing.Converters {
    public class InverseBool : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return !(bool) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return !(bool) value;
        }
    }
}