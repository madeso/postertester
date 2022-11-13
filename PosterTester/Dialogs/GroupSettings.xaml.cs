using System;
using System.Windows;
using System.Windows.Input;

using Data = PosterLib.Data;
namespace PosterTester.Dialogs;

/// <summary>
/// Interaction logic for GroupSettings.xaml
/// </summary>
public partial class GroupSettings : Window
{
	public GroupSettings(Data.RequestGroup group)
	{
		InitializeComponent();
		this.DataContext = group;
	}

	private void Window_Activated(object sender, EventArgs e)
	{
		this.dlgCount.SelectAll();
		this.dlgCount.Focus();
	}

	private void OkExecuted(object sender, ExecutedRoutedEventArgs e)
	{
		this.DialogResult = true;
		Close();
	}
}
