using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PosterLib.Data;

public class Time : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	public long TotalMilliSeconds
	{
		get
		{
			long r = Minutes;

			// minutes to seconds
			r = r * 60 + Seconds;

			// seconds to milli
			r = r * 1000 + MilliSeconds;

			// return milli
			return r;
		}

		set
		{
			long r = value;

			static int Div(ref long val, long div)
			{
				var x = val % div;
				var y = (val-x) / div;

				val = y;
				return (int)x;
			}

			MilliSeconds = Div(ref r, 1000);
			Seconds = Div(ref r, 60);
			Minutes = (int)r;
		}
	}

	public int MilliSeconds { get; set; }
	public int Seconds { get; set; }
	public int Minutes { get; set; }

	public override string ToString()
	{
		return $"{Minutes:00}:{Seconds:00}:{MilliSeconds:000}";
	}
}
