using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace PostTestr.Data;

public class Request : INotifyPropertyChanged
{
    private HttpMethod _method = HttpMethod.Get;
    private string _url = "http://localhost:8080/";
    private string _title = "";
    private string _textConent = string.Empty;
    private Response _response = null;
    private bool _isWorking = false;
    private ContentType _contentType = ContentType.Json;

    private string _titleOrUrl = "";
    private bool _hasPost = false;

    public string TitleOrUrl
    {
        get => _titleOrUrl; private set
        {
            _titleOrUrl = value;
            OnPropertyChanged();
        }
    }
    void UpdateTitleOrUrl()
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
        get => _hasPost; set
        {
            _hasPost = value;
            OnPropertyChanged();
        }
    }
    private void UpdateHasPost()
    {
        this.HasPost = Logic.HasContent(this.Method);
    }

    public string Url
    {
        get => _url; set
        {
            _url = value;
            OnPropertyChanged();
            UpdateTitleOrUrl();
        }
    }

    public string Title
    {
        get => _title; set
        {
            _title = value;
            OnPropertyChanged();
            UpdateTitleOrUrl();
        }
    }

    public HttpMethod Method
    {
        get => _method; set
        {
            _method = value;
            OnPropertyChanged();
            UpdateHasPost();
        }
    }

    public ContentType ContentType
    {
        get => _contentType; set
        {
            _contentType = value;
            OnPropertyChanged();
        }
    }

    public string TextContent
    {
        get => _textConent; set
        {
            _textConent = value;
            OnPropertyChanged();
        }
    }

    public Response Response
    {
        get => _response; set
        {
            _response = value;
            OnPropertyChanged();
        }
    }

    public bool IsWorking
    {
        get => _isWorking; set
        {
            _isWorking = value;
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
        return this.ContentType switch
        {
            ContentType.Json => Logic.WithJsonContent(this.TextContent),
            ContentType.Text => Logic.WithStringContent(this.TextContent),
            _ => throw new Exception($"Invalid type ${this.ContentType}"),
        };
    }
}
