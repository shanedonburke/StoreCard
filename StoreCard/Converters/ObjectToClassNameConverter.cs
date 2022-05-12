﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace StoreCard.Converters;

public class ObjectToClassNameConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value.GetType().Name;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}