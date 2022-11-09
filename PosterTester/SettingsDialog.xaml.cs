using System;
using System.Windows;
using System.Windows.Input;

namespace PosterTester;


/// <summary>
/// Interaction logic for Settings.xaml
/// </summary>
public partial class SettingsDialog : Window
{
	public SettingsDialog(Data.Data theData)
	{
		InitializeComponent();
		this.DataContext = theData;
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
