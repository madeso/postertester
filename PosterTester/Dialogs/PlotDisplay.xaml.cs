using System;
using System.Windows;

namespace PosterTester;

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
