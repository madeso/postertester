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
        get => _body; set
        {
            _body = value;
            OnPropertyChanged();
        }
    }

    public TimeSpan Time
    {
        get => _time; internal set
        {
            _time = value;
            OnPropertyChanged();
        }
    }

    public Response(HttpStatusCode status, string body)
    {
        Status = status;
        Body = body;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }


}
