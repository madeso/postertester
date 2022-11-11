using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Effects;
using Microsoft.Win32;
using PosterTester.Data;
using ScottPlot;

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

    public Data.Root Root { get; internal set; }

    public MainWindow()
    {
        InitializeComponent();

		this.Root = Disk.LoadOrCreateNew();
		this.DataContext = this.Root;
		this.Closed += (closedSender, closedArgs) => Save();

		// https://scottplot.net/faq/mvvm/
		this.Root.OnSelectionChanged += () => UpdatePlotForSelectedRequest();

		UpdatePlotForSelectedRequest();
	}

    private void Save()
    {
		Disk.Save(this.Root);
	}

    private Data.Request GetSelectedRequest()
    {
        var g = this.Root.SelectedGroup;
        if (g == null) { return null; }
        return g.SelectedRequest;
    }

	public void BrowseDataExecuted(object sender, ExecutedRoutedEventArgs e)
	{
		Logic.BrowseFolder(PosterTester.Data.Disk.GetAppFolder());
	}

	private void ShowError(string err)
	{
		// MessageBox.Show(this, err, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		var dlg = new Dialogs.ExMessageBox(err, "Error", SystemIcons.Error);
		using var blur = new DialogBackground(this, dlg);
		dlg.ShowDialog();
	}

	public async void ExecuteExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        var r = GetSelectedRequest();
        if (r == null) { ShowMissingRequest(); return; }

		dlgMainTab.SelectedItem = dlgTabResponse;
		await Logic.Request(this.Root, r);
        Save();
    }

	private void ShowMissingRequest()
	{
		ShowError("No request is selected!");
	}

	public void SettingsExecuted(object sender, ExecutedRoutedEventArgs e)
	{
		// todo(Gustav): move settings to a custom class so we can clone and update...
		var oldBin = this.Root.BinSize;
		var dlg = new SettingsDialog(this.Root);
		using var blur = new DialogBackground(this, dlg);
		if (dlg.ShowDialog() ?? false)
		{
			Save();
			if(oldBin != this.Root.BinSize)
			{
				// bin size was updated: update plot
				UpdatePlotForSelectedRequest();
			}
		}
		else
		{
			this.Root.BinSize = oldBin;
		}
	}

	public void GroupSettingsExecuted(object sender, ExecutedRoutedEventArgs e)
	{
		var group = this.Root.SelectedGroup;
		if(group == null) { ShowMissingGroup(); return; }
		var oldName = group.Name;
		var dlg = new GroupSettings(group);
		using var blur = new DialogBackground(this, dlg);
		if (dlg.ShowDialog() ?? false)
		{
			Save();
		}
		else
		{
			group.Name = oldName;
		}
	}

	private void ShowMissingGroup()
	{
		ShowError("No group is selected!");
	}

	private Data.AttackOptions RunAttackDialog()
	{
		var dlg = new AttackDialog(this.Root.Attack.Clone());
		using var blur = new DialogBackground(this, dlg);
		if (dlg.ShowDialog() ?? false)
		{
			return dlg.AttackOptions;
		}
		else
		{
			return null;
		}
	}

	private async void AttackExecuted(object sender, ExecutedRoutedEventArgs e)
	{
		var r = GetSelectedRequest();
		if (r == null) { ShowMissingRequest(); return; }

		var options = RunAttackDialog();
		if (options != null)
		{
			this.Root.Attack.GetFrom(options);
			Save();
			dlgMainTab.SelectedItem = dlgTabAttack;
			var result = await Logic.Attack(this.Root, r);

			// todo(Gustav): display error!
			r.AttackResult = result;
			r.AttackOptions = options;
			UpdatePlotForRequest(r);
			// Save();
		}
	}

	private void UpdatePlotForRequest(Request r)
	{
		Plotter.Plot(this.dlgPlot, r, this.Root.BinSize);
	}

	private void UpdatePlotForSelectedRequest()
	{
		UpdatePlotForRequest(this.Root.SelectedGroup == null ? null : this.Root.SelectedGroup.SelectedRequest);
	}

	public void CreateNewGroupExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        var fdlg = new SaveFileDialog();
        fdlg.Title = "Create new group";
        fdlg.Filter = PostGroupFilesFilter;
        fdlg.RestoreDirectory = true;
        fdlg.DefaultExt = PostGroupExt;
        bool? dr = fdlg.ShowDialog();
        if (dr == false) { return; }

        this.Root.CreateNewGroup(fdlg.FileName);
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

        this.Root.AddExistingGroup(fdlg.FileName);
        Save();
    }

    public void ForgetGroupExecuted(object sender, ExecutedRoutedEventArgs e)
    {
		var group = Root.SelectedGroup;
		if (group == null) { ShowMissingGroup(); return; }
		if (group.Builtin) { ShowError("You can't forget the builtin group");  return; }

        this.Root.ForgetGroup();
        Save();
    }

    private void FormatExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        var r = GetSelectedRequest();
        if (r == null) { ShowMissingRequest(); return; }

        r.TextContent = Logic.FormatJsonOrNot(r.TextContent);
        r.Response.Body = Logic.FormatJsonOrNot(r.Response.Body);
        Save();
    }

    private void NewRequestExecuted(object sender, ExecutedRoutedEventArgs e)
    {
		if(this.Root.SelectedGroup != null) { ShowMissingGroup(); return; }

		this.Root.AddNewRequest();
		Save();
    }

    private void ExitExecuted(object sender, ExecutedRoutedEventArgs e)
    {
		Save();
        Close();
    }

    private void DeleteSelectedRequestExecuted(object sender, ExecutedRoutedEventArgs e)
    {
		var r = GetSelectedRequest();
		if (r == null) { ShowMissingRequest(); return; }

		var removed = this.Root.DeleteSelectedRequest();
		if(removed == false)
		{
			ShowError("Unable to remove the last request!");
			return;
		}

        Save();
    }

    private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        Save();
    }

	private void RenameExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        var r = GetSelectedRequest();
        if (r == null) { ShowMissingRequest(); return; }

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
        var dlg = new CompareRequests(this.Root);
        using var blur = new DialogBackground(this, dlg);
        if (dlg.ShowDialog() ?? false)
        {
            this.Root.Compare();
        }
    }

	private void CompareAttackExecuted(object sender, ExecutedRoutedEventArgs e)
	{
		var dlg = new CompareRequests(this.Root);
		using var blur = new DialogBackground(this, dlg);
		if (dlg.ShowDialog() ?? false)
		{
			var displayDialog = true;
			var dialog = new PlotDisplay(plot => {
				var msg = Plotter.ComparePlot(plot, this.Root.LeftCompare, this.Root.RightCompare, this.Root.BinSize);
				if (msg != null)
				{
					MessageBox.Show(msg, "Plott error", MessageBoxButton.OK, MessageBoxImage.Error);
					displayDialog = false;
				}
			});

			if(displayDialog)
			{
				dialog.ShowDialog();
			}
		}
	}

	private void LoadPostExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        var r = GetSelectedRequest();
        if (r == null)
        {
			ShowMissingRequest();
            return;
        }


		var fdlg = new OpenFileDialog
		{
			Title = "Browse post data",
			Filter = "Text files (json,txt,xml)|*.json;*.txt;*.xml|All files (*.*)|*.*",
			RestoreDirectory = true
		};
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
        if (r == null) { ShowMissingRequest(); return; }

        this.uiPost.Focus();
        this.uiPost.SelectAll();
    }

    private void SelectRequest(int i1)
    {
        var g = this.Root.SelectedGroup;
        if (g == null) { ShowMissingGroup(); return; }

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
