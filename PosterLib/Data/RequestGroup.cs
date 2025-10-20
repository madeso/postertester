using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using PosterLib.Domain;
using PosterLib.Saved;

namespace PosterLib.Data;

public class AuthData
{
	public string BearerToken { get; set; } = string.Empty;
}

public class RequestGroup : INotifyPropertyChanged
{
	private bool _builtin = false;
	private string _name = "";
	private string _file = "";
	private string _bearerToken = "";
	private string _baseUrl = "";
	private bool _useBaseUrl = false;

	private Request? selectedRequest = null;
	private ObservableCollection<Request> requests = new();
	private Guid guid;

	public ObservableCollection<Request> Requests
	{
		get => this.requests; set
		{
			this.requests = value;
			OnPropertyChanged();
		}
	}

	public Request? SelectedRequest
	{
		get => this.selectedRequest; set
		{
			this.selectedRequest = value;
			OnPropertyChanged();
			OnSelectionChanged();
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

	public bool Builtin
	{
		get => this._builtin; set
		{
			this._builtin = value;
			OnPropertyChanged();
		}
	}

	public string Name
	{
		get => this._name; set
		{
			this._name = value;
			OnPropertyChanged();
		}
	}

	public string File
	{
		get => this._file; set
		{
			this._file = value;
			OnPropertyChanged();
		}
	}

	public string BearerToken
	{
		get => this._bearerToken; set
		{
			this._bearerToken = value;
			OnPropertyChanged();
		}
	}

	public string BaseUrl
	{
		get => this._baseUrl; set
		{
			this._baseUrl = value;
			OnPropertyChanged();
		}
	}

	public bool UseBaseUrl
	{
		get => this._useBaseUrl; set
		{
			this._useBaseUrl = value;
			OnPropertyChanged();
		}
	}

	public Action OnSelectionChanged { get; set; } = () => { };

	public void AddNewRequest()
	{
		var r = new Request() { Guid = Guid.NewGuid() };
		if (this.SelectedRequest != null)
		{
			var newUrl = new UriBuilder(new Uri(this.SelectedRequest.Url))
			{
				Path = "",
				Query = ""
			};
			r.Url = newUrl.Uri.AbsoluteUri;
		}
		this.Requests.Add(r);
		this.SelectedRequest = r;
	}

	// return the next selected request (or null)
	public Request? DeleteSelectedRequest()
	{
		if (this.SelectedRequest == null) { return null; }
		if (this.Requests.Count <= 1) { return null; }

		int index = this.Requests.IndexOf(this.SelectedRequest);
		this.Requests.RemoveAt(index);
		var nextRequest = this.Requests.Count > 0 ? this.Requests[Math.Min(index, this.Requests.Count - 1)]
			: null;

		this.SelectedRequest = nextRequest;

		return nextRequest;
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	protected void OnPropertyChanged([CallerMemberName] string? name = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}

	Request? RequestFromName(string name)
		=> this.Requests.FirstOrDefault(r => r.Title == name);

	public void ImportPostman(string path)
	{
		var postman = Disk.ReadFile<PostmanFile>(path);

		var vars = postman.Variables.ToImmutableDictionary(x => x.Key, x => x.Value);

		foreach (var src in postman.Items)
		{
			var request = RequestFromName(src.Name);
			if(request == null)
			{
				request = new Request() { Guid = Guid.NewGuid(), Title = src.Name };
				this.Requests.Add(request);
			}

			request.Method = Enum.TryParse<HttpMethod>(src.Request.Method, true, out var method)
				? method
				: HttpMethod.Get;
			request.Url = src.Request.Url.Raw;
			if (vars.Count > 0)
			{
				foreach (var v in vars)
				{
					request.Url = request.Url.Replace("{{" + v.Key + "}}", v.Value);
				}
			}
			if (src.Request.Body.Mode == "raw")
			{
				request.TextContent = src.Request.Body.Raw;
				request.ContentType = ContentTypeJson.Instance;
			}
			else
			{
				request.TextContent = "";
				request.ContentType = ContentTypeText.Instance;
			}
		}
	}

	public void MoveRequestUp()
	{
		var r = this.SelectedRequest;
		if (r == null) return;
		int index = this.Requests.IndexOf(r);
		this.Requests.RemoveAt(index);
		this.Requests.Insert(Math.Max(0, index - 1), r);
		this.SelectedRequest = r;
	}

	public void MoveRequestDown()
	{
		var r = this.SelectedRequest;
		if (r == null) return;
		int index = this.Requests.IndexOf(r);
		this.Requests.RemoveAt(index);
		this.Requests.Insert(Math.Min(this.Requests.Count, index + 1), r);
		this.SelectedRequest = r;
	}

	public void SortRequests()
	{
		var r = this.SelectedRequest;
		if (r == null) return;
		// todo(Gustav): provide a comparer so the order is defined
		var sorted = this.Requests.All(x => string.IsNullOrWhiteSpace(x.Title))
			? this.Requests.OrderBy(x => x.Url)
			: this.Requests.OrderBy(x => x.TitleOrUrl);
		this.Requests = new ObservableCollection<Request>(sorted);
		this.SelectedRequest = r;
	}

	public AuthData ToAuthData()
	{
		return new AuthData { BearerToken = this.BearerToken };
	}

	public GroupSettings ToGroupSettings()
	{
		return new GroupSettings(this.UseBaseUrl, this.BaseUrl);
	}

	public void EnableBaseUrl()
	{
		if (this.Builtin) return;

		var lefts = Requests.Select(r => new Uri(r.Url)).Where(x=>x.IsAbsoluteUri).Select(GetBaseUrl).ToImmutableSortedSet();

		foreach (var request in Requests)
		{
			var originalUrl = new Uri(request.Url);
			if (originalUrl.IsAbsoluteUri == false) continue;
			var baseUrl = new Uri(GetBaseUrl(originalUrl));
			var local = baseUrl.MakeRelativeUri(originalUrl);
			var newUrl = local.ToString();
			request.Url = newUrl;
		}

		this.UseBaseUrl = true;
		this.BaseUrl = lefts.Count == 1 ? lefts[0] : "";
	}

	private string GetBaseUrl(Uri x)
	{
		return x.GetLeftPart(UriPartial.Authority);
	}

	public void DisableBaseUrl()
	{
		if(this.Builtin) return;

		foreach (var request in this.Requests)
		{
			if(Uri.IsWellFormedUriString(request.Url, UriKind.Absolute) && new Uri(request.Url).IsAbsoluteUri)
			{
				// do nothing if for some reason the base url is not used for this request
				continue;
			}

			request.Url = Logic.JoinUrlString(this.BaseUrl, request);
		}
		this.UseBaseUrl = false;
		this.BaseUrl = "";
	}
}


