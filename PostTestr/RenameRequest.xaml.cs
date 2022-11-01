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

    public string NewDisplay { get => this.newDisplay; set { this.newDisplay = value; OnPropertyChanged(); } }
    public string OldDisplay { get => this.oldDisplay; set { this.oldDisplay = value; OnPropertyChanged(); } }
    public string Title { get => this.title; set { this.title = value; OnPropertyChanged(); UpdateTitle(); } }

    private void UpdateTitle()
    {
        this.NewDisplay = Data.Request.CalculateDisplay(this.Url, this.Title);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}


public partial class RenameRequest : Window
{
    private readonly RenameRequestData data;
    public RenameRequest(Data.Request r)
    {
        InitializeComponent();
        this.data = new RenameRequestData()
        {
            Url = r.Url,
            OldDisplay = r.TitleOrUrl,
            Title = r.Title
        };
        this.DataContext = this.data;
    }

    public string RequestTitle
    {
        get => this.data.Title;
    }

    private void Window_Activated(object sender, EventArgs e)
    {
        this.dlgTitle.SelectAll();
        this.dlgTitle.Focus();
    }

    private void OkExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        this.DialogResult = true;
        Close();
    }
}
