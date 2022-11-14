namespace PosterTester.Converters;

using System;
using System.Globalization;
using System.Windows.Data;
using PosterLib.Domain;

public class TimeSpanConverter : IValueConverter
{
    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null) { return string.Empty; }
		if (value is not TimeSpan) { return string.Empty; }
		var t = (TimeSpan)value;

		return Logic.TimeSpanToString(t);
	}

	public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
