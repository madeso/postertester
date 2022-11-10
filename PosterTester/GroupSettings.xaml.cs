using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PosterTester;

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
