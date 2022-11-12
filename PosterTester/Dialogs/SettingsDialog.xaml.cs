using System;
using System.Windows;
using System.Windows.Input;

namespace PosterTester.Dialogs;


/// <summary>
/// Interaction logic for Settings.xaml
/// </summary>
public partial class SettingsDialog : Window
{
	public SettingsDialog(Data.Root root)
	{
		InitializeComponent();
		this.DataContext = root;
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
