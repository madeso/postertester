using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PosterTester.Data;


public class RequestGroup : INotifyPropertyChanged
{
	private bool _builtin = false;
	private string _name = "";
	private string _file = "";

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
}
