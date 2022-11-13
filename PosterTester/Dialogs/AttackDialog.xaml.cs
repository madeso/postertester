using System;
using System.Windows;
using System.Windows.Input;
using PosterLib.Data;

namespace PosterTester.Dialogs;

/// <summary>
/// Interaction logic for AttackDialog.xaml
/// </summary>
public partial class AttackDialog : Window
{
	public AttackDialog(AttackOptions attackOptions)
	{
		InitializeComponent();
		this.AttackOptions = attackOptions;
		this.DataContext = attackOptions;
	}

	public AttackOptions AttackOptions { get; }

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
