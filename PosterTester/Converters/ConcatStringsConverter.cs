namespace PosterTester.Converters;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Data;


public class ConcatStringsConverter : IValueConverter
{
	public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null) { return string.Empty; }
		if (value is Collection<string> collection)
		{
			var line = new string('=', 120);
			var nl = "\r\n";
			return string.Join($"{nl}{nl}{line}{nl}{nl}", collection);
		}
		return string.Empty;
	}

	public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
