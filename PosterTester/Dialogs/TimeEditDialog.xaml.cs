using System;
using System.Windows;
using PosterLib.Data;

namespace PosterTester.Dialogs;

/// <summary>
/// Interaction logic for TimeSpanEditDialog.xaml
/// </summary>
public partial class TimeEditDialog : Window
{
	private readonly Time _time;

	public TimeEditDialog(Time d)
	{
		InitializeComponent();
		_time = d;
		this.DataContext = _time;
	}

	private void TimeSpanEditDialog_OnActivated(object sender, EventArgs e)
	{
		this.dlgInitial.SelectAll();
		dlgInitial.Focus();
	}

	private void OkExecuted(object sender, EventArgs e)
	{
		this.DialogResult = true;
		this.Close();
	}
}
