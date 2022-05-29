#region

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#endregion

namespace StoreCard.Converters;

// From https://stackoverflow.com/a/1039681
[ValueConversion(typeof(bool), typeof(Visibility))]
public sealed class InverseBooleanToVisibilityConverter : IValueConverter
{
    #region IValueConverter Members

    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType != typeof(Visibility))
        {
            throw new InvalidOperationException("The target must be a visibility setting.");
        }

        if (value is not bool b)
        {
            throw new InvalidOperationException("The given value is not a bool.");
        }

        return b ? Visibility.Hidden : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();

    #endregion
}
