using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
namespace PostTestr.Data;

public class Response : INotifyPropertyChanged
{
    // public HttpStatusCode Status { get; set; }
    private string _body;

    public HttpStatusCode Status { get; }

    public string Body
    {
        get => _body; set
        {
            _body = value;
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
