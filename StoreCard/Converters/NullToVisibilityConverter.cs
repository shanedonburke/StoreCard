#region

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#endregion

namespace StoreCard.Converters;

/// <summary>
/// Converts a value to a <see cref="Visibility"/>.
/// If the value is null, "Collapsed" is returned.
/// Otherwise, "Visible" is returned.
/// </summary>
[ValueConversion(typeof(object), typeof(Visibility))]
public sealed class NullToVisibilityConverter : IValueConverter
{
    #region IValueConverter Members

    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture) =>
        value == null ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();

    #endregion
}
