using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace PosterTester.Dialogs;

public class RenameRequestData : INotifyPropertyChanged
{
    private string _newDisplay;
    private string _oldDisplay;
    private string _title;

	public RenameRequestData(string oldDisplay, string title, string url)
	{
		this._newDisplay = Data.Request.CalculateDisplay(url, title);
		this._oldDisplay = oldDisplay;
		this._title = title;
		this.Url = url;
	}

	public string Url { get; set; }

    public string NewDisplay { get => this._newDisplay; set { this._newDisplay = value; OnPropertyChanged(); } }
    public string OldDisplay { get => this._oldDisplay; set { this._oldDisplay = value; OnPropertyChanged(); } }
    public string Title { get => this._title; set { this._title = value; OnPropertyChanged(); UpdateTitle(); } }

    private void UpdateTitle()
    {
        this.NewDisplay = Data.Request.CalculateDisplay(this.Url, this.Title);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
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
        this.data = new RenameRequestData(
            url: r.Url,
            oldDisplay: r.TitleOrUrl,
            title: r.Title
        );
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
