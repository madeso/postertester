namespace PostTestr.Converters;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

public class TimeSpanConverter : IValueConverter
{
    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) { return string.Empty; }
        if (value is not TimeSpan) { return string.Empty; }
        var t = (TimeSpan)value;

        static int Floor(double totalMinutes)
        {
            return (int)Math.Floor(totalMinutes);
        }

        static string S(int x)
        {
            if (x == 0) { return string.Empty; }
            else { return "s"; }
        }

        var r = new List<string>();

        if (t.TotalMinutes > 1)
        {
            int min = Floor(t.TotalMinutes);
            r.Add($"{min} minute{S(min)}");
            t = t.Subtract(TimeSpan.FromMinutes(min));
        }

        if (t.TotalSeconds > 1)
        {
            int min = Floor(t.TotalSeconds);
            r.Add($"{min} second{S(min)}");
            t = t.Subtract(TimeSpan.FromSeconds(min));
        }

        if (t.TotalMilliseconds > 1)
        {
            int min = Floor(t.TotalMilliseconds);
            r.Add($"{min} millisecond{S(min)}");
        }

        if (r.Count == 0)
        {
            return "Less than 1 ms";
        }
        else
        {
            return string.Join(" ", r);
        }
    }

    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
