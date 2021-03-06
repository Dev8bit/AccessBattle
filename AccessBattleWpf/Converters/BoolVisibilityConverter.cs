﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AccessBattle.Wpf.Converters
{
    [Obsolete("Use built-in BooleanToVisibilityConverter class")]
    public class BoolVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value)
                    return Visibility.Visible;
            }
            if (value is bool?)
            {
                if ((bool?)value == true)
                    return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is Visibility && (Visibility)value == Visibility.Visible);
        }
    }
}
