using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ExchangeTracker.Presentation.Common
{
    public class ContentVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PercentBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = double.Parse(value.ToString());
            return v > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value == null ? string.Empty : value.ToString();
            if (val.Length == 1)
                switch (val[0])
                {
                    case 'A':
                        return "مجاز";
                    case 'I':
                        return "غیر مجاز";
                    default:
                        return val[0];
                }
            return "-";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
