using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace PostTestr;

public class RenameRequestData : INotifyPropertyChanged
{
    private string newDisplay;
    private string oldDisplay;
    private string title;

    public string Url { get; set; }

    public string NewDisplay { get => newDisplay; set { newDisplay = value; OnPropertyChanged(); } }
    public string OldDisplay { get => oldDisplay; set { oldDisplay = value; OnPropertyChanged(); } }
    public string Title { get => title; set { title = value; OnPropertyChanged(); UpdateTitle(); } }

    void UpdateTitle()
    {
        NewDisplay = Data.Request.CalculateDisplay(this.Url, this.Title);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}


public partial class RenameRequest : Window
{
    readonly RenameRequestData data;
    public RenameRequest(Data.Request r)
    {
        InitializeComponent();
        data = new RenameRequestData()
        {
            Url = r.Url,
            OldDisplay = r.TitleOrUrl,
            Title = r.Title
        };
        this.DataContext = data;
    }

    public string RequestTitle
    {
        get => data.Title;
    }

    private void Window_Activated(object sender, EventArgs e)
    {
        dlgTitle.SelectAll();
        dlgTitle.Focus();
    }

    private void OkExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}
