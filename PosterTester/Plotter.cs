using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScottPlot;
using ScottPlot.Control.EventProcess.Events;

namespace PosterTester;

internal static class Plotter
{
	public static void Plot(WpfPlot wpf, double[] values, Data.AttackOptions attack)
	{
		var plt = wpf.Plot;
		plt.Clear();

		var mi = values.Min();
		var ma = values.Max();

		// create a histogram
		(double[] counts, double[] binEdges) = ScottPlot.Statistics.Common.Histogram(values, min: mi, max: ma, binSize: 0.02);
		double[] leftEdges = binEdges.Take(binEdges.Length - 1).ToArray();

		// display histogram probabability as a bar plot
		var bar = plt.AddBar(values: counts, positions: leftEdges);
		bar.FillColor = ColorTranslator.FromHtml("#9bc3eb");
		bar.BorderLineWidth = 0;

		// display histogram distribution curve as a line plot on a secondary Y axis
		double[] smoothEdges = ScottPlot.DataGen.Range(start: binEdges.First(), stop: binEdges.Last(), step: 0.1, includeStop: true);
		double[] smoothDensities = ScottPlot.Statistics.Common.ProbabilityDensity(values, smoothEdges, percent: true);
		var probPlot = plt.AddScatterLines(
			xs: smoothEdges,
			ys: smoothDensities,
			lineWidth: 2,
			label: "probability");
		probPlot.YAxisIndex = 1;
		plt.YAxis2.Ticks(true);

		// display vertical lines at points of interest
		var stats = new ScottPlot.Statistics.BasicStats(values);

		plt.AddVerticalLine(stats.Mean, Color.Black, 2, LineStyle.Solid, "mean");

		plt.AddVerticalLine(stats.Mean - stats.StDev, Color.Black, 2, LineStyle.Dash, "1 SD");
		plt.AddVerticalLine(stats.Mean + stats.StDev, Color.Black, 2, LineStyle.Dash);

		plt.AddVerticalLine(stats.Mean - stats.StDev * 2, Color.Black, 2, LineStyle.Dot, "2 SD");
		plt.AddVerticalLine(stats.Mean + stats.StDev * 2, Color.Black, 2, LineStyle.Dot);

		plt.AddVerticalLine(stats.Min, Color.Gray, 1, LineStyle.Dash, "min/max");
		plt.AddVerticalLine(stats.Max, Color.Gray, 1, LineStyle.Dash);

		plt.Legend(location: Alignment.UpperRight);

		// customize the plot style
		var sorp = attack.AtTheSameTime ? "Paralell" : "Serial";
		plt.Title($"{sorp} attack of {attack.Count}");
		plt.YAxis.Label("Count (#)");
		plt.YAxis2.Label("Probability (%)");
		plt.XAxis.Label("Time taken (s)");
		plt.SetAxisLimits(yMin: 0);
		plt.SetAxisLimits(yMin: 0, yAxisIndex: 1);

		wpf.Refresh();
	}
}
