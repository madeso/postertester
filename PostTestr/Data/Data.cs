﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;

namespace PostTestr.Data;

public class RequestGroup : INotifyPropertyChanged
{
    private Request selectedRequest = null;
    private ObservableCollection<Request> requests = new ObservableCollection<Request>();

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

    // return the next selected request (or null)
    public Request DeleteSelectedRequest()
    {
        if (this.SelectedRequest == null) { return null; }
        if (this.Requests.Count <= 1) { return null; }

        var index = this.Requests.IndexOf(this.SelectedRequest);
        this.Requests.RemoveAt(index);
        var nextRequest = this.Requests.Count > 0 ? this.Requests[Math.Min(index, this.Requests.Count - 1)] 
            : null;

        this.SelectedRequest = nextRequest;

        return nextRequest;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}



public class Data : INotifyPropertyChanged
{
    private Request leftCompare = null;
    private Request rightCompare = null;
    private CookieContainer cookies;
    private RequestGroup selectedRequest = null;
    private ObservableCollection<RequestGroup> requests = new ObservableCollection<RequestGroup>();

    public ObservableCollection<RequestGroup> Groups
    {
        get => requests; set
        {
            requests = value;
            OnPropertyChanged();
        }
    }

    public RequestGroup SelectedGroup
    {
        get => selectedRequest; set
        {
            selectedRequest = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<HttpMethod> AllRequestMethods { get; } = new ObservableCollection<HttpMethod>
    {
        HttpMethod.Get,
        HttpMethod.Post,
        HttpMethod.Put,
        HttpMethod.Delete,
        HttpMethod.Patch
    };

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
        var g = this.SelectedGroup;
        if(g == null) { return; }

        g.AddNewRequest();
    }

    public void DeleteSelectedRequest()
    {
        var g = this.SelectedGroup;
        if (g == null) { return; }

        var previous = g.SelectedRequest;
        var next = g.DeleteSelectedRequest();

        if (this.LeftCompare == previous)
        {
            this.LeftCompare = next;
        }

        if (this.RightCompare == previous)
        {
            this.RightCompare = next;
        }
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
