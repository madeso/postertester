using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

namespace PosterTester.Data;

public class Headers
{
	public class Row
	{
		public Row(string name, string[] values)
		{
			this.Name = name;
			this.Values = values;
		}

		public string Name { get; private set; }
		public string[] Values { get; private set; }
	}

	public Row[] Rows { get; set; } = Array.Empty<Row>();

	internal static Headers Collect(HttpResponseHeaders src)
	{
		return new Headers { Rows = CollectToRows(src).ToArray() };
	}

	internal static Headers Collect(HttpContentHeaders src)
	{
		return new Headers { Rows = CollectToRows(src).ToArray() };
	}

	private static IEnumerable<Row> CollectToRows<T>(T src)
		where T : IEnumerable<KeyValuePair<string, IEnumerable<string>>>
	{
		foreach (var e in src)
		{
			yield return new Row(e.Key, e.Value.ToArray());
		}
	}
}

public class Response : INotifyPropertyChanged
{
	private string _body;
	private TimeSpan _time;
	private Headers responseHeaders;
	private Request? parentRequest;

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

	public Request? ParentRequest
	{
		get => parentRequest; internal set
		{
			parentRequest = value;
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
