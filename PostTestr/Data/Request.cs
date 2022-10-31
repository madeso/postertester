using Newtonsoft.Json;
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
    private string _response = string.Empty;
    private bool _isWorking = false;
    private ContentType _contentType = ContentType.Json;

    private string _titleOrUrl = "";
    private bool _hasPost = false;

    [JsonIgnore]
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

    [JsonIgnore]
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

    [JsonProperty("url")]
    public string Url
    {
        get => _url; set
        {
            _url = value;
            OnPropertyChanged();
            UpdateTitleOrUrl();
        }
    }

    [JsonProperty("title")]
    public string Title
    {
        get => _title; set
        {
            _title = value;
            OnPropertyChanged();
            UpdateTitleOrUrl();
        }
    }

    [JsonProperty("method")]
    public HttpMethod Method
    {
        get => _method; set
        {
            _method = value;
            OnPropertyChanged();
            UpdateHasPost();
        }
    }

    [JsonProperty("post_type")]
    public ContentType ContentType
    {
        get => _contentType; set
        {
            _contentType = value;
            OnPropertyChanged();
        }
    }

    [JsonProperty("post")]
    public string TextContent
    {
        get => _textConent; set
        {
            _textConent = value;
            OnPropertyChanged();
        }
    }

    [JsonIgnore]
    public string Response
    {
        get => _response; set
        {
            _response = value;
            OnPropertyChanged();
        }
    }

    [JsonIgnore]
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
