using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;

namespace PostTestr.Data;

public class RequestGroup : INotifyPropertyChanged
{
    private bool _builtin;
    private string _name;
    private string _file;

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

    public bool Builtin
    {
        get => _builtin; set
        {
            _builtin = value;
            OnPropertyChanged();
        }
    }

    public string Name
    {
        get => _name; set
        {
            _name = value;
            OnPropertyChanged();
        }
    }

    public string File
    {
        get => _file; set
        {
            _file = value;
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
    private RequestGroup selectedRequest = null;
    private RequestGroup leftGroup = null;
    private RequestGroup rightGroup = null;
    private ObservableCollection<RequestGroup> requests = new ObservableCollection<RequestGroup>();
    private bool _formatResponse = true;

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

    public RequestGroup LeftGroup
    {
        get => leftGroup; set
        {
            leftGroup = value;
            OnPropertyChanged();
            LeftCompare = value.Requests[0];
        }
    }

    public RequestGroup RightGroup
    {
        get => rightGroup; set
        {
            rightGroup = value;
            OnPropertyChanged();
            RightCompare = value.Requests[0];
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

    public bool FormatResponse
    {
        get => _formatResponse; set
        {
            _formatResponse = value;
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

    internal void CreateBuiltinIfMissing()
    {
        var hasBuiltin = Groups.Where(x => x.Builtin).Any();
        if (hasBuiltin) { return; }

        var g = new RequestGroup
        {
            Builtin = true,
            Name = "My requests",
            File = Disk.RequestsFile
        };

        g.AddNewRequest();

        this.Groups.Add(g);
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
        var gitfolder = Path.Join(info.Directory.FullName, ".git");
        var name = Path.GetFileNameWithoutExtension(fileName);
        if(new DirectoryInfo(gitfolder).Exists)
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
        if(this.SelectedGroup == null) { return; }
        if (this.SelectedGroup.Builtin) { return; }

        Disk.SaveGroup(this.SelectedGroup);

        var index = this.Groups.IndexOf(this.SelectedGroup);
        this.Groups.Remove(this.SelectedGroup);
        index = Math.Min(index, this.Groups.Count-1);
        this.SelectedGroup = this.Groups[index];
    }
}
