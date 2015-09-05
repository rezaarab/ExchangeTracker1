using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

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

    public class ValueSignToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double result;
            if (double.TryParse(value != null ? value.ToString() : string.Empty, out result))
                return result > 0;
            return true;
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
            var val = value == null ? string.Empty : value.ToString().Trim().ToUpper();
            switch (val)
            {
                case "A":
                    return "مجاز";
                case "AR":
                    return "مجاز-محفوظ";
                case "IS":
                    return "ممنوع-متوقف";
                default:
                    return val;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TimeSpanToBackGround : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan)
            {
                var item = (TimeSpan)value;
                if (item.Seconds == 0)// data updated
                    return Brushes.CadetBlue;
                if (item.Seconds == 1)//last record
                    return Brushes.Tomato;
                if (item.Seconds == 2)//empty record
                    return Brushes.Transparent;
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
