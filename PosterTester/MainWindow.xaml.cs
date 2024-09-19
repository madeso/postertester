using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using PosterLib.Data;
using PosterLib.Domain;
using ScottPlot;

namespace PosterTester;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const string PostGroupExt = "PosterTesterGroup";
    private const string PostGroupFilesFilter = $"Post group files (*.{PostGroupExt})|*.{PostGroupExt}|All files (*.*)|*.*";

    public Root Root { get; internal set; }

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

    private Request? GetSelectedRequest()
    {
        var g = this.Root.SelectedGroup;
        if (g == null) { return null; }
        return g.SelectedRequest;
    }

	public void BrowseDataExecuted(object sender, ExecutedRoutedEventArgs e)
	{
		Browser.BrowseFolder(Disk.GetAppFolder());
	}

	private void ShowError(string err)
	{
		var dlg = new Dialogs.ExMessageBox(err, "Error", SystemIcons.Error);
		using var blur = new DialogBackgroundWithDialog(this, dlg);
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
		var dlg = new Dialogs.SettingsDialog(this.Root);
		using var blur = new DialogBackgroundWithDialog(this, dlg);
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
		var dlg = new Dialogs.GroupSettings(group);
		using var blur = new DialogBackgroundWithDialog(this, dlg);
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

	private void BrowseUrlExecuted(object sender, ExecutedRoutedEventArgs e)
	{
		string? RunBrowseDialog()
		{
			var url = this.Root.SelectedGroup?.SelectedRequest?.Url;
			if (url == null) return null;

			var dlg = new Dialogs.BrowseUrl(url);
			using var blur = new DialogBackgroundWithDialog(this, dlg);
			if (dlg.ShowDialog() ?? false)
			{
				return url;
			}
			else
			{
				return null;
			}
		}

		RunBrowseDialog();
	}


	private void AbortExecuted(object sender, ExecutedRoutedEventArgs e)
	{
		var req = GetSelectedRequest();
		if (req == null) return;

		var cancel = req.CancellationToken;
		if(cancel == null) return;

		if(cancel.IsCancellationRequested) return;
		cancel.Cancel();
	}

	private void ChangeTimeoutExecuted(object sender, ExecutedRoutedEventArgs e)
	{
		Time? RunChangeTimeoutDialog(Request r)
		{
			var time = new Time { TotalMilliSeconds = r.Timeout.TotalMilliSeconds };
			var dlg = new Dialogs.TimeEditDialog(time);
			using var blur = new DialogBackgroundWithDialog(this, dlg);
			if (dlg.ShowDialog() ?? false)
			{
				return time;
			}
			else
			{
				return null;
			}
		}

		var req = GetSelectedRequest();
		if (req == null) return;
		
		var time = RunChangeTimeoutDialog(req);
		if(time == null) return;

		req.Timeout = time;
	}

	private async void AttackExecuted(object sender, ExecutedRoutedEventArgs e)
	{
		AttackOptions? RunAttackDialog()
		{
			var dlg = new Dialogs.AttackDialog(this.Root.Attack.Clone());
			using var blur = new DialogBackgroundWithDialog(this, dlg);
			if (dlg.ShowDialog() ?? false)
			{
				return dlg.AttackOptions;
			}
			else
			{
				return null;
			}
		}

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

	private void UpdatePlotForRequest(Request? r)
	{
		if(r== null) { return; }
		Plotter.Plot(this.dlgPlot, r, this.Root.BinSize);
	}

	private void UpdatePlotForSelectedRequest()
	{
		UpdatePlotForRequest(this.Root.SelectedGroup == null ? null : this.Root.SelectedGroup.SelectedRequest);
	}

	public void CreateNewGroupExecuted(object sender, ExecutedRoutedEventArgs e)
    {
		var fdlg = new SaveFileDialog
		{
			Title = "Create new group",
			Filter = PostGroupFilesFilter,
			RestoreDirectory = true,
			DefaultExt = PostGroupExt
		};

		using var blur = new DialogBackground(this);
		if (fdlg.ShowDialog() == false) { return; }

        this.Root.CreateNewGroup(fdlg.FileName);
        Save();
    }

    public void AddExistingGroupExecuted(object sender, ExecutedRoutedEventArgs e)
    {
		var fdlg = new OpenFileDialog
		{
			Title = "Select existing group file",
			Filter = PostGroupFilesFilter,
			RestoreDirectory = true
		};
		using var blur = new DialogBackground(this);
		if (fdlg.ShowDialog() == false) { return; }

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
		if(r.Response != null)
		{
			r.Response.Body = Logic.FormatJsonOrNot(r.Response.Body);
		}
		Save();
    }

    private void NewRequestExecuted(object sender, ExecutedRoutedEventArgs e)
    {
		if(this.Root.SelectedGroup == null) { ShowMissingGroup(); return; }

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

        var dlg = new Dialogs.RenameRequest(r);
        using var blur = new DialogBackgroundWithDialog(this, dlg);
        if (dlg.ShowDialog() ?? false)
        {
            r.Title = dlg.RequestTitle;
            Save();
        }
    }

    private void CompareExecuted(object sender, ExecutedRoutedEventArgs e)
    {
		string? error = null;
		{
			var dlg = new Dialogs.CompareRequests(this.Root);
			using var blur = new DialogBackgroundWithDialog(this, dlg);
			if (dlg.ShowDialog() ?? false)
			{
				error = DiffTool.Compare(this.Root);
			}
		}

		if (error != null)
		{
			ShowError(error);
		}
	}

	private void CompareAttackExecuted(object sender, ExecutedRoutedEventArgs e)
	{
		var compareRequestsDialog = new Dialogs.CompareRequests(this.Root);
		using var blur = new DialogBackgroundWithDialog(this, compareRequestsDialog);
		if (compareRequestsDialog.ShowDialog() ?? false)
		{
			var displayDialog = true;
			var plotDisplayDialog = new Dialogs.PlotDisplay(plot => {
				var msg = Plotter.ComparePlot(plot, this.Root.LeftCompare, this.Root.RightCompare, this.Root.BinSize);
				if (msg != null)
				{
					MessageBox.Show(msg, "Plott error", MessageBoxButton.OK, MessageBoxImage.Error);
					displayDialog = false;
				}
			});

			if(displayDialog)
			{
				plotDisplayDialog.ShowDialog();
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

		using var blur = new DialogBackground(this);
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
