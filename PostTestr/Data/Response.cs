using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
namespace PostTestr.Data;

public class Response : INotifyPropertyChanged
{
    private string _body;
    private TimeSpan _time;

    public HttpStatusCode Status { get; }

    public string Body
    {
        get => this._body; set
        {
            this._body = value;
            OnPropertyChanged();
        }
    }

    public TimeSpan Time
    {
        get => this._time; internal set
        {
            this._time = value;
            OnPropertyChanged();
        }
    }

    public Response(HttpStatusCode status, string body)
    {
        this.Status = status;
        this.Body = body;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }


}
