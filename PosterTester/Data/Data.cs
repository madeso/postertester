using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PosterTester.Data;

public class RequestGroup : INotifyPropertyChanged
{
    private bool _builtin;
    private string _name;
    private string _file;

    private Request selectedRequest = null;
    private ObservableCollection<Request> requests = new ObservableCollection<Request>();

    public ObservableCollection<Request> Requests
    {
        get => this.requests; set
        {
            this.requests = value;
            OnPropertyChanged();
        }
    }

    public Request SelectedRequest
    {
        get => this.selectedRequest; set
        {
            this.selectedRequest = value;
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

        int index = this.Requests.IndexOf(this.SelectedRequest);
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
    private RequestGroup selectedRequest = null;
    private RequestGroup leftGroup = null;
    private RequestGroup rightGroup = null;
    private ObservableCollection<RequestGroup> requests = new ObservableCollection<RequestGroup>();
    private bool _formatResponse = true;

    public ObservableCollection<RequestGroup> Groups
    {
        get => this.requests; set
        {
            this.requests = value;
            OnPropertyChanged();
        }
    }

    public RequestGroup SelectedGroup
    {
        get => this.selectedRequest; set
        {
            this.selectedRequest = value;
            OnPropertyChanged();
        }
    }

    public RequestGroup LeftGroup
    {
        get => this.leftGroup; set
        {
            this.leftGroup = value;
            OnPropertyChanged();
            this.LeftCompare = value?.Requests[0];
        }
    }

    public RequestGroup RightGroup
    {
        get => this.rightGroup; set
        {
            this.rightGroup = value;
            OnPropertyChanged();
            this.RightCompare = value?.Requests[0];
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

    public ObservableCollection<ContentType> AllContentTypes { get; } = new ObservableCollection<ContentType>
    {
        ContentTypeJson.Instance,
        ContentTypeText.Instance
    };

    public Request LeftCompare
    {
        get => this.leftCompare; set
        {
            this.leftCompare = value;
            OnPropertyChanged();
        }
    }

    public Request RightCompare
    {
        get => this.rightCompare; set
        {
            this.rightCompare = value;
            OnPropertyChanged();
        }
    }

    public bool FormatResponse
    {
        get => this._formatResponse; set
        {
            this._formatResponse = value;
            OnPropertyChanged();
        }
    }

    public void AddNewRequest()
    {
        var g = this.SelectedGroup;
        if (g == null) { return; }

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

    internal void CreateBuiltinIfMissing()
    {
        bool hasBuiltin = this.Groups.Where(x => x.Builtin).Any();
        if (hasBuiltin) { return; }

        var g = new RequestGroup
        {
            Builtin = true,
            Name = "My requests",
            File = Disk.RequestsFile
        };

        this.Groups.Add(g);
		this.SelectedGroup = g;
    }

    internal void CreateNewGroup(string fileName)
    {
        var g = new RequestGroup();
        g.Name = GuessGroupName(fileName);
        g.File = fileName;
        g.Builtin = false;
        g.AddNewRequest();
        this.Groups.Add(g);
        this.SelectedGroup = g;
    }

    private static string GuessGroupName(string fileName)
    {
        var info = new FileInfo(fileName);
        string gitfolder = Path.Join(info.Directory.FullName, ".git");
        string name = Path.GetFileNameWithoutExtension(fileName);
        if (new DirectoryInfo(gitfolder).Exists)
        {
            return $"{name} for {info.Directory.Name}";
        }
        else
        {
            return name;
        }
    }

    internal void AddExistingGroup(string fileName)
    {
        var g = new RequestGroup();
        g.Requests = Disk.LoadRequests(fileName);
        g.Name = GuessGroupName(fileName);
        g.File = fileName;
        g.Builtin = false;
        g.SelectedRequest = g.Requests[0];
        this.Groups.Add(g);
        this.SelectedGroup = g;
    }

    internal void ForgetGroup()
    {
        if (this.SelectedGroup == null) { return; }
        if (this.SelectedGroup.Builtin) { return; }

        Disk.SaveGroup(this.SelectedGroup);

        int index = this.Groups.IndexOf(this.SelectedGroup);
        this.Groups.Remove(this.SelectedGroup);
        index = Math.Min(index, this.Groups.Count - 1);
        this.SelectedGroup = this.Groups[index];
    }
}
