#region

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#endregion

namespace StoreCard.Converters;

/// <summary>
/// Converts a value to a <see cref="Visibility"/>.
/// If the given value is a string that contains non-whitespace characters, "Visible" is returned.
/// Otherwise, "Collapsed" is returned.
/// </summary>
[ValueConversion(typeof(string), typeof(Visibility))]
public sealed class NotBlankToVisibilityConverter : IValueConverter
{
    #region IValueConverter Members

    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            return str.Trim() == string.Empty ? Visibility.Collapsed : Visibility.Visible;
        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();

    #endregion
}
