using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using PosterLib.Domain;

namespace PosterLib.Data;

public class Request : INotifyPropertyChanged
{
	public const long DefaultTimeoutMs = 100000;
	public const HttpMethod DefaultMethod = HttpMethod.Get;
	public const string DefaultUrl = "http://localhost:8080/";
	public const string DefaultTitle = "";

	private HttpMethod _method = DefaultMethod;
	private string _url = DefaultUrl;
	private string _title = DefaultTitle;
	private string _textContent = string.Empty;
	private Response? _response = null;
	private bool _isWorking = false;
	private ContentType _contentType = ContentTypeJson.Instance;
	private Time _timeout = new Time{TotalMilliSeconds = DefaultTimeoutMs};
	private string _titleOrUrl = "";
	private bool _hasPost = false;

	private AttackResult? attackResult;
	private AttackOptions? attackOptions;
	private Guid guid;
	private string plotStatus = string.Empty;

	public CancellationTokenSource? CancellationToken { get; set; } = null;

	public string TitleOrUrl
	{
		get => this._titleOrUrl; private set
		{
			this._titleOrUrl = value;
			OnPropertyChanged();
		}
	}

	public Time Timeout
	{
		get => this._timeout;
		set
		{
			this._timeout = value;
			OnPropertyChanged();
		}
	}

	public Guid Guid
	{
		get => guid; set
		{
			guid = value;
			OnPropertyChanged();
		}
	}

	private void UpdateTitleOrUrl()
	{
		this.TitleOrUrl = CalculateDisplay();
	}

	public Request()
	{
		UpdateTitleOrUrl();
	}

	private string CalculateDisplay()
	{
		return CalculateDisplay(this.Url, this.Title);
	}

	public static string CalculateDisplay(string url, string title)
	{
		if (string.IsNullOrWhiteSpace(title))
		{
			return url;
		}
		else
		{
			return $"{title} ({url})";
		}
	}

	public bool HasPost
	{
		get => this._hasPost; private set
		{
			this._hasPost = value;
			OnPropertyChanged();
		}
	}
	private void UpdateHasPost()
	{
		this.HasPost = Logic.HasContent(this.Method);
	}

	public string Url
	{
		get => this._url; set
		{
			this._url = value;
			OnPropertyChanged();
			UpdateTitleOrUrl();
		}
	}

	public string Title
	{
		get => this._title; set
		{
			this._title = value;
			OnPropertyChanged();
			UpdateTitleOrUrl();
		}
	}

	public HttpMethod Method
	{
		get => this._method; set
		{
			this._method = value;
			OnPropertyChanged();
			UpdateHasPost();
		}
	}

	public ContentType ContentType
	{
		get => this._contentType; set
		{
			this._contentType = value;
			OnPropertyChanged();
		}
	}

	public string TextContent
	{
		get => this._textContent; set
		{
			this._textContent = value;
			OnPropertyChanged();
		}
	}

	public Response? Response
	{
		get => this._response; set
		{
			this._response = value;
			OnPropertyChanged();
		}
	}

	public bool IsWorking
	{
		get => this._isWorking; set
		{
			this._isWorking = value;
			OnPropertyChanged();
		}
	}

	public AttackResult? AttackResult
	{
		get => attackResult; set
		{
			attackResult = value;
			OnPropertyChanged();
		}
	}

	public AttackOptions? AttackOptions
	{
		get => attackOptions; set
		{
			attackOptions = value;
			OnPropertyChanged();
		}
	}

	public string PlotStatus
	{
		get => plotStatus; set
		{
			plotStatus = value;
			OnPropertyChanged();
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	protected void OnPropertyChanged([CallerMemberName] string? name = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}

	internal System.Net.Http.HttpContent GetContent()
	{
		return this.ContentType.CreateContent(this);
	}
}

