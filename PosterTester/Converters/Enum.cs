namespace PosterTester.Converters;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

public class UnderlyingEnumConverter : IValueConverter
{
    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) { return string.Empty; }
        if (value is not Enum) { return string.Empty; }
        var t = (int)value;

        return t;
    }

    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
