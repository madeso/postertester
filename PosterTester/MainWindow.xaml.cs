using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Effects;
using Microsoft.Win32;

namespace PosterTester;

public class DialogBackground : IDisposable
{
    private Window Parent { get; }
    public DialogBackground(Window parent, Window dialog)
    {
        this.Parent = parent;

        parent.Opacity = 0.5;
        parent.Effect = new BlurEffect();

        dialog.Owner = parent;
        dialog.ShowInTaskbar = false;
        dialog.Topmost = true;
    }

    public void Dispose()
    {
        this.Parent.Opacity = 1;
        this.Parent.Effect = null;
    }
}

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const string PostGroupExt = "PosterTesterGroup";
    private const string PostGroupFilesFilter = $"Post group files (*.{PostGroupExt})|*.{PostGroupExt}|All files (*.*)|*.*";


    public Data.Data Data { get; internal set; }

    public MainWindow()
    {
        InitializeComponent();
    }

    public Action OnSave { get; set; } = () => { };

    private void Save()
    {
        this.OnSave?.Invoke();
    }

    private Data.Request GetSelectedRequest()
    {
        var g = this.Data.SelectedGroup;
        if (g == null) { return null; }
        return g.SelectedRequest;
    }

	public void BrowseDataExecuted(object sender, ExecutedRoutedEventArgs e)
	{
		Logic.BrowseFolder(PosterTester.Data.Disk.GetAppFolder());
	}

	public async void ExecuteExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        var r = GetSelectedRequest();
        if (r == null)
        {
            return;
        }
        await Logic.Request(this.Data, r);
        Save();
    }

    public void CreateNewGroupExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        var fdlg = new SaveFileDialog();
        fdlg.Title = "Crete post data";
        fdlg.Filter = PostGroupFilesFilter;
        fdlg.RestoreDirectory = true;
        fdlg.DefaultExt = PostGroupExt;
        bool? dr = fdlg.ShowDialog();
        if (dr == false) { return; }

        this.Data.CreateNewGroup(fdlg.FileName);
        Save();
    }

    public void AddExistingGroupExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        var fdlg = new OpenFileDialog();
        fdlg.Title = "Select existing group file";
        fdlg.Filter = PostGroupFilesFilter;
        fdlg.RestoreDirectory = true;
        bool? dr = fdlg.ShowDialog();
        if (dr == false) { return; }

        this.Data.AddExistingGroup(fdlg.FileName);
        Save();
    }

    public void ForgetGroupExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        this.Data.ForgetGroup();
        Save();
    }

    private void FormatExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        var r = GetSelectedRequest();
        if (r == null)
        {
            return;
        }
        r.TextContent = Logic.FormatJsonOrNot(r.TextContent);
        r.Response.Body = Logic.FormatJsonOrNot(r.Response.Body);
        Save();
    }

    private void NewRequestExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        this.Data.AddNewRequest();
        Save();
    }

    private void ExitExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        Close();
    }

    private void DeleteSelectedRequestExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        this.Data.DeleteSelectedRequest();
        Save();
    }

    private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        Save();
    }

    private void RenameExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        var r = GetSelectedRequest();
        if (r == null) { return; }

        var dlg = new RenameRequest(r);
        using var blur = new DialogBackground(this, dlg);
        if (dlg.ShowDialog() ?? false)
        {
            r.Title = dlg.RequestTitle;
            Save();
        }
    }

    private void CompareExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        var dlg = new CompareRequests(this.Data);
        using var blur = new DialogBackground(this, dlg);
        if (dlg.ShowDialog() ?? false)
        {
            this.Data.Compare();
        }
    }

    private void LoadPostExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        var r = GetSelectedRequest();
        if (r == null)
        {
            return;
        }


        var fdlg = new OpenFileDialog();
        fdlg.Title = "Browse post data";
        fdlg.Filter = "Text files (json,txt,xml)|*.json;*.txt;*.xml|All files (*.*)|*.*";
        fdlg.RestoreDirectory = true;
        bool? dr = fdlg.ShowDialog();
        if (dr == false) { return; }

        r.TextContent = File.ReadAllText(fdlg.FileName);
    }

    private void FocusRequestsExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        this.uiRequests.Focus();
    }

    private void FocusUrlExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        this.uiUrl.Focus();
        this.uiUrl.SelectAll();
    }

    private void FocusPostExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        var r = GetSelectedRequest();
        if (r == null)
        {
            return;
        }
        this.uiPost.Focus();
        this.uiPost.SelectAll();
    }

    private void SelectRequest(int i1)
    {
        var g = this.Data.SelectedGroup;
        if (g == null) { return; }

        int i = i1 - 1;
        if (i < 0) return;
        if (i >= g.Requests.Count) return;
        g.SelectedRequest = g.Requests[i];
    }

    private void SelectRequest1Executed(object sender, ExecutedRoutedEventArgs e)
    {
        SelectRequest(1);
    }

    private void SelectRequest2Executed(object sender, ExecutedRoutedEventArgs e)
    {
        SelectRequest(2);
    }

    private void SelectRequest3Executed(object sender, ExecutedRoutedEventArgs e)
    {
        SelectRequest(3);
    }

    private void SelectRequest4Executed(object sender, ExecutedRoutedEventArgs e)
    {
        SelectRequest(4);
    }

    private void SelectRequest5Executed(object sender, ExecutedRoutedEventArgs e)
    {
        SelectRequest(5);
    }

    private void SelectRequest6Executed(object sender, ExecutedRoutedEventArgs e)
    {
        SelectRequest(6);
    }

    private void SelectRequest7Executed(object sender, ExecutedRoutedEventArgs e)
    {
        SelectRequest(7);
    }

    private void SelectRequest8Executed(object sender, ExecutedRoutedEventArgs e)
    {
        SelectRequest(8);
    }

    private void SelectRequest9Executed(object sender, ExecutedRoutedEventArgs e)
    {
        SelectRequest(9);
    }
}