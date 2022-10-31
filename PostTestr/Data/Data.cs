using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;

namespace PostTestr.Data;

public class Data : INotifyPropertyChanged
{
    private Request selectedRequest = null;
    private Request leftCompare = null;
    private Request rightCompare = null;

    private CookieContainer cookies;
    private ObservableCollection<Request> requests = new ObservableCollection<Request>();

    public ObservableCollection<HttpMethod> AllRequestMethods { get; } = new ObservableCollection<HttpMethod>
    {
        HttpMethod.Get,
        HttpMethod.Post,
        HttpMethod.Put,
        HttpMethod.Delete,
        HttpMethod.Patch
    };

    public ObservableCollection<Request> Requests
    {
        get => requests; set
        {
            requests = value;
            OnPropertyChanged();
        }
    }

    public Request SelectedRequest
    {
        get => selectedRequest; set
        {
            selectedRequest = value;
            OnPropertyChanged();
        }
    }

    public Request LeftCompare
    {
        get => leftCompare; set
        {
            leftCompare = value;
            OnPropertyChanged();
        }
    }

    public Request RightCompare
    {
        get => rightCompare; set
        {
            rightCompare = value;
            OnPropertyChanged();
        }
    }

    public CookieContainer Cookies
    {
        get => cookies; set
        {
            cookies = value;
            OnPropertyChanged();
        }
    }

    public void AddNewRequest()
    {
        var r = new Request();
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

    public void DeleteSelectedRequest()
    {
        if (this.SelectedRequest == null) { return; }
        if (this.Requests.Count <= 1) { return; }

        var index = this.Requests.IndexOf(this.SelectedRequest);
        this.Requests.RemoveAt(index);
        var nextRequest = this.Requests[Math.Min(index, this.Requests.Count - 1)];

        if(this.LeftCompare == this.SelectedRequest)
        {
            this.LeftCompare = nextRequest;
        }

        if (this.RightCompare == this.SelectedRequest)
        {
            this.RightCompare = nextRequest;
        }

        this.SelectedRequest = nextRequest;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    internal void Compare()
    {
        var lhs = this.LeftCompare;
        var rhs = this.RightCompare;
        if (lhs == null || rhs == null)
        {
            return;
        }

        DiffTool.LaunchDiff(lhs.Response, rhs.Response);
    }
}
