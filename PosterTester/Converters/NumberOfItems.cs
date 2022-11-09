namespace PosterTester.Converters;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

public class NumberOfItemsConverter : IValueConverter
{
	public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null) { return string.Empty; }
		if (value is ICollection collection)
		{
			return $"{collection.Count}";
		}
		return string.Empty;
	}

	public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
