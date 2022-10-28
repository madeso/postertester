using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
namespace PostTestr;


public enum ContentType
{
    [EnumMember(Value = "json")]
    Json,

    [EnumMember(Value = "text")]
    Text,
}

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
        if(string.IsNullOrWhiteSpace(this.Title))
        {
            this.TitleOrUrl = this.Url;
        }
        else
        {
            this.TitleOrUrl = $"{this.Title} ({this.Url})";
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

    [JsonProperty("response")]
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

public class RequestFile
{
    [JsonProperty("requests")]
    public Request[] Requests { get; set; }

    [JsonProperty("selected_request")]
    public int SelectedRequest { get; set; }

    [JsonProperty("left_compare")]
    public int LeftCompare { get; set; }

    [JsonProperty("right_compare")]
    public int RightCompare { get; set; }
}

public static class Disk
{
    private static string SettingsFile
    {
        get
        {
            var root = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var folder = Path.Combine(root, "PostTestr");
            if(Directory.Exists(folder) == false)
            {
                Directory.CreateDirectory(folder);
            }
            return Path.Combine(folder, "settings.json"); ;
        }
    }

    private static Data Load(string file)
    {
        var data = File.ReadAllText(file);
        var container = JsonConvert.DeserializeObject<RequestFile>(data);
        var requests = new ObservableCollection<Request>(container.Requests);
        return new Data
        {
            Requests = requests,
            SelectedRequest = requests[container.SelectedRequest],
            LeftCompare = requests[container.LeftCompare],
            RightCompare  = requests[container.RightCompare]
        };
    }

    private static void Save(Data data, string file)
    {
        var jsonFile = new RequestFile
        {
            Requests = data.Requests.ToArray(),
            SelectedRequest = data.Requests.IndexOf(data.SelectedRequest),
            LeftCompare = data.Requests.IndexOf(data.LeftCompare),
            RightCompare = data.Requests.IndexOf(data.RightCompare)
        };
        var jsonData = JsonConvert.SerializeObject(jsonFile, Formatting.Indented);
        File.WriteAllText(file, jsonData);
    }

    public static Data LoadOrCreateNew()
    {
        if (File.Exists(SettingsFile))
        {
            return Load(SettingsFile);
        }
        else
        {
            var r = new Data { };
            r.AddNewRequest();
            return r;
        }
    }

    public static void Save(Data data)
    {
        Save(data, SettingsFile);
    }
}
