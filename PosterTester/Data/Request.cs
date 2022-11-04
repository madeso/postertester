﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace PosterTester.Data;

public class Request : INotifyPropertyChanged
{
    private HttpMethod _method = HttpMethod.Get;
    private string _url = "http://localhost:8080/";
    private string _title = "";
    private string _textConent = string.Empty;
    private Response _response = null;
    private bool _isWorking = false;
    private ContentType _contentType = ContentTypeJson.Instance;

    private string _titleOrUrl = "";
    private bool _hasPost = false;

    public string TitleOrUrl
    {
        get => this._titleOrUrl; private set
        {
            this._titleOrUrl = value;
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
        get => this._hasPost; set
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
        get => this._textConent; set
        {
            this._textConent = value;
            OnPropertyChanged();
        }
    }

    public Response Response
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

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    internal System.Net.Http.HttpContent GetContent()
    {
        return this.ContentType.CreateContent(this);
    }
}