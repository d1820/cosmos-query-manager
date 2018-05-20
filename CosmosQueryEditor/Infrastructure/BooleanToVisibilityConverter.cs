using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using MaterialDesignThemes.Wpf.Converters;

namespace CosmosQueryEditor.Infrastructure
{
    public class BooleanToVisibilityConverter2 : IValueConverter
    {
        public Visibility TrueValue { get; set; } = Visibility.Visible;
        public Visibility FalseValue { get; set; } = Visibility.Collapsed;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bValue = false;
            if (value is bool)
            {
                bValue = (bool) value;
            }

            return bValue ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value as Visibility? == TrueValue;
        }
    }

    public static class StaticMarkup
    {
        public static readonly BooleanToVisibilityConverter BooleanToVisibilityConverter = new BooleanToVisibilityConverter();
    }
}