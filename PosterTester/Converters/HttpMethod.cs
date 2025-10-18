using System;
using System.Globalization;
using System.Windows.Data;
using PosterLib.Domain;

namespace PosterTester.Converters;

public class HttpMethodConverter<T> :  IValueConverter
{
	public T? Get {get; set;} = default(T);
	public T? Post {get; set;} = default(T);
	public T? Put {get; set;} = default(T);
	public T? Delete {get; set;} = default(T);
	public T? Patch { get; set; } = default(T);

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		var method = value as HttpMethod?;
		if (method == null) { return this.Get; }
		return method.Value switch
		{
			HttpMethod.Get => this.Get,
			HttpMethod.Post => this.Post,
			HttpMethod.Put => this.Put,
			HttpMethod.Delete => this.Delete,
			HttpMethod.Patch => this.Patch,
			_ => this.Get,
		};
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}

public class HttpMethodImageConverter : HttpMethodConverter<System.Windows.Media.Imaging.BitmapImage>
{
}
