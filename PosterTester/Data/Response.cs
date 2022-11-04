using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
namespace PosterTester.Data;

public class HeaderRow
{
	public string Name { get; set; }
	public string[] Values { get; set; }
}

public class Headers
{
	public HeaderRow[] Rows { get; set; } = Array.Empty<HeaderRow>();

	internal static Headers Collect(HttpResponseHeaders src)
	{
		return new Headers { Rows = CollectToRows(src).ToArray() };
	}

	internal static Headers Collect(HttpContentHeaders src)
	{
		return new Headers { Rows = CollectToRows(src).ToArray() };
	}

	private static IEnumerable<HeaderRow> CollectToRows<T>(T src)
		where T : IEnumerable<KeyValuePair<string, IEnumerable<string>>>
	{
		foreach (var e in src)
		{
			yield return new HeaderRow { Name = e.Key, Values = e.Value.ToArray() };
		}
	}
}

public class Response : INotifyPropertyChanged
{
	private string _body;
	private TimeSpan _time;
	private Headers responseHeaders;

	public HttpStatusCode Status { get; }

	public string Body
	{
		get => this._body; set
		{
			this._body = value;
			OnPropertyChanged();
		}
	}

	public Headers ResponseHeaders
	{
		get => responseHeaders; set
		{
			responseHeaders = value;
			OnPropertyChanged();
		}
	}

	public TimeSpan Time
	{
		get => this._time; internal set
		{
			this._time = value;
			OnPropertyChanged();
		}
	}

	public Response(HttpStatusCode status, string body, Headers responseHeaders)
	{
		this.Status = status;
		this.Body = body;
		this.ResponseHeaders = responseHeaders;
	}


	public event PropertyChangedEventHandler PropertyChanged;

	protected void OnPropertyChanged([CallerMemberName] string name = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}
}
