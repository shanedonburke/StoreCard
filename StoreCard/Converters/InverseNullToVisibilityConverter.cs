#region

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#endregion

namespace StoreCard.Converters;

/// <summary>
/// Converts a value to a <see cref="Visibility"/>.
/// If the value is null, "Visible" is returned.
/// Otherwise, "Collapsed is returned.
/// </summary>
[ValueConversion(typeof(object), typeof(Visibility))]
public sealed class InverseNullToVisibilityConverter : IValueConverter
{
    #region IValueConverter Members

    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture) =>
        value == null ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();

    #endregion
}
