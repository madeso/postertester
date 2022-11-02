namespace PosterTester.Converters;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;


public class BooleanConverter<T> : IValueConverter
{
    public BooleanConverter(T trueValue, T falseValue)
    {
        this.True = trueValue;
        this.False = falseValue;
    }

    public T True { get; set; }
    public T False { get; set; }

    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool && ((bool)value) ? this.True : this.False;
    }

    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is T coverted && EqualityComparer<T>.Default.Equals(coverted, this.True);
    }
}


public sealed class BooleanToVisibilityConverter : BooleanConverter<Visibility>
{
    public BooleanToVisibilityConverter() :
        base(Visibility.Visible, Visibility.Collapsed)
    { }
}
