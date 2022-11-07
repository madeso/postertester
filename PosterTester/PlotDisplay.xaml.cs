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

namespace PosterTester
{
	/// <summary>
	/// Interaction logic for PlotDisplay.xaml
	/// </summary>
	public partial class PlotDisplay : Window
	{
		public PlotDisplay(Action<ScottPlot.Plot> plotFunction)
		{
			InitializeComponent();
			plotFunction(dlgPlot.Plot);
			dlgPlot.Refresh();
		}
	}
}
