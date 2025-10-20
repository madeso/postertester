using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

namespace PosterLib.Data;

public class Headers
{
	public class Row(string name, string[] values)
	{
		public string Name { get; private set; } = name;
		public string[] Values { get; private set; } = values;
	}

	public Row[] Rows { get; set; } = [];

	public static Headers Join(params Headers[] headers)
		=> new() { Rows = headers.SelectMany(x => x.Rows).ToArray() };

	internal static Headers Collect(HttpResponseHeaders src)
		=> new() { Rows = CollectToRows(src).ToArray() };

	internal static Headers Collect(HttpContentHeaders src)
		=> new() { Rows = CollectToRows(src).ToArray() };

	private static IEnumerable<Row> CollectToRows<T>(T src)
		where T : IEnumerable<KeyValuePair<string, IEnumerable<string>>>
		=> src.Select(e => new Row(e.Key, e.Value.ToArray()));
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
		this._body = body;
		this.responseHeaders = responseHeaders;
	}


	public event PropertyChangedEventHandler? PropertyChanged;

	protected void OnPropertyChanged([CallerMemberName] string? name = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}
}
