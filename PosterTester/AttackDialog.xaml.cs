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
using PosterTester.Data;

namespace PosterTester
{
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
}
