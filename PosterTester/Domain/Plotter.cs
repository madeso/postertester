using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PosterTester.Data;
using ScottPlot;
using ScottPlot.Control.EventProcess.Events;

namespace PosterTester.Domain;

internal static class Plotter
{
	private const int PROBABILITY_ALPHA = 250;
	private const int BAR_ALPHA = 200;

	// bang-wong palette
	// private static readonly Color BLACK = Color.FromArgb(0, 0, 0);
	private static readonly Color ORANGE = Color.FromArgb(230, 159, 0);
	private static readonly Color SKY_BLUE = Color.FromArgb(86, 180, 233);
	private static readonly Color BLUEISH_GREEN = Color.FromArgb(0, 158, 115);
	private static readonly Color YELLOW = Color.FromArgb(240, 228, 66);
	private static readonly Color BLUE = Color.FromArgb(0, 114, 178);
	private static readonly Color VERMILLION = Color.FromArgb(213, 94, 0);
	private static readonly Color REDDISH_PURPLE = Color.FromArgb(204, 121, 167);

	private static readonly Color[] COLORS = new Color[]
	{
		ORANGE,
		SKY_BLUE,
		BLUEISH_GREEN,
		YELLOW,
		BLUE,
		VERMILLION,
		REDDISH_PURPLE
	};

	private static int GetColorIndex(Request rq)
	{
		var arr = rq.Guid.ToByteArray();
		int r = 0;
		foreach (var b in arr)
		{
			r += b;
		}
		return r % COLORS.Length;
	}

	public static void Plot(WpfPlot wpf, Request r, double binSize)
	{
		var plt = wpf.Plot;

		r.PlotStatus = string.Empty;

		CleanPlot(plt);
		var status = SinglePlot(r, plt, binSize);

		wpf.Refresh();

		if (status != null)
		{
			r.PlotStatus = status;
		}
	}

	private static void CleanPlot(Plot plt)
	{
		plt.Clear();
		plt.Title(string.Empty);
		plt.YAxis.Label(string.Empty);
		plt.YAxis2.Label(string.Empty);
		plt.XAxis.Label(string.Empty);
	}

	private static string? SinglePlot(Request r, Plot plt, double binSize)
	{
		if (CanPlot(r) == false) { return "Nothing to plot"; }

		var attack = r.AttackOptions!;

		var color = COLORS[GetColorIndex(r)];

		var values = ExtractPlottable(r);

		plt.Palette = Palette.Microcharts;

		var mi = values.Min();
		var ma = values.Max();

		var diff = ma - mi;
		if (diff < binSize)
		{
			return $"Bin size needs to be less than {diff}";
		}

		// create a histogram
		(double[] counts, double[] binEdges) = ScottPlot.Statistics.Common.Histogram(values, min: mi, max: ma, binSize: binSize);
		double[] leftEdges = binEdges.Take(binEdges.Length - 1).ToArray();

		// display histogram probabability as a bar plot
		var bar = plt.AddBar(values: counts, positions: leftEdges);
		bar.BorderLineWidth = 0;
		bar.BarWidth = binSize;
		bar.FillColor = Color.FromArgb(BAR_ALPHA, color);

		// display histogram distribution curve as a line plot on a secondary Y axis
		double[] smoothEdges = DataGen.Range(start: binEdges.First(), stop: binEdges.Last(), step: 0.1, includeStop: true);
		double[] smoothDensities = ScottPlot.Statistics.Common.ProbabilityDensity(values, smoothEdges, percent: true);
		var probPlot = plt.AddScatterLines(
			xs: smoothEdges,
			ys: smoothDensities,
			lineWidth: 2,
			color: Color.FromArgb(PROBABILITY_ALPHA, color),
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
		var sorp = attack.AtTheSameTime ? "Parallel" : "Serial";
		plt.Title($"{sorp} attack of {attack.Count}");
		plt.YAxis.Label("Count (#)");
		plt.YAxis2.Label("Probability (%)");
		plt.XAxis.Label("Time taken (ms)");
		plt.SetAxisLimits(yMin: 0);
		plt.SetAxisLimits(yMin: 0, yAxisIndex: 1);

		return null;
	}

	private static double[] ExtractPlottable(Request r)
	{
		return r.AttackResult!.Result.Select(x => x.TotalMilliseconds).ToArray();
	}

	internal static string? ComparePlot(Plot plt, Request? leftCompare, Request? rightCompare, double binSize)
	{
		if (CanPlot(leftCompare) == false || CanPlot(rightCompare) == false) { return "Nothing to plot"; }
		else return PleaseComparePlot(plt, leftCompare!, rightCompare!, binSize);
	}

	internal static string? PleaseComparePlot(Plot plt, Request leftCompare, Request rightCompare, double binSize)
	{
		var heightsMale = ExtractPlottable(leftCompare);
		var heightsFemale = ExtractPlottable(rightCompare);

		plt.Palette = Palette.Microcharts;

		var mi = heightsFemale.Concat(heightsMale).Min();
		var ma = heightsFemale.Concat(heightsMale).Max();

		var diff = ma - mi;
		if (diff < binSize)
		{
			return $"Bin size needs to be less than {diff}";
		}

		// calculate histograms for male and female datasets
		(double[] probMale, double[] binEdges) = ScottPlot.Statistics.Common.Histogram(heightsMale, min: mi, max: ma, binSize: binSize, density: false);
		(double[] probFemale, _) = ScottPlot.Statistics.Common.Histogram(heightsFemale, min: mi, max: ma, binSize: binSize, density: false);
		double[] leftEdges = binEdges.Take(binEdges.Length - 1).ToArray();

		// plot histograms
		var barMale = plt.AddBar(values: probMale, positions: leftEdges);
		// barMale.BarWidth = 1;
		barMale.BorderLineWidth = 0;

		var barFemale = plt.AddBar(values: probFemale, positions: leftEdges);
		// barFemale.BarWidth = 1;
		barFemale.BorderLineWidth = 0;

		var leftColorIndex = GetColorIndex(leftCompare);
		var rightColorIndex = GetColorIndex(rightCompare);
		if (rightColorIndex == leftColorIndex)
		{
			rightColorIndex = (rightColorIndex + 1) % COLORS.Length;
		}

		barMale.BarWidth = binSize;
		barMale.FillColor = Color.FromArgb(BAR_ALPHA, COLORS[leftColorIndex]);
		barFemale.BarWidth = binSize;
		barFemale.FillColor = Color.FromArgb(BAR_ALPHA, COLORS[rightColorIndex]);

		// plot probability function curves
		double[] pdfMale = ScottPlot.Statistics.Common.ProbabilityDensity(heightsMale, binEdges, percent: true);
		plt.AddScatterLines(
			xs: binEdges,
			ys: pdfMale,
			color: Color.FromArgb(PROBABILITY_ALPHA, COLORS[leftColorIndex]),
			lineWidth: 3,
			label: $"{leftCompare.TitleOrUrl} (n={heightsMale.Length:N0})");

		double[] pdfFemale = ScottPlot.Statistics.Common.ProbabilityDensity(heightsFemale, binEdges, percent: true);
		plt.AddScatterLines(
			xs: binEdges,
			ys: pdfFemale,
			color: Color.FromArgb(PROBABILITY_ALPHA, COLORS[rightColorIndex]),
			lineWidth: 3,
			label: $"{rightCompare.TitleOrUrl} (n={heightsFemale.Length:N0})");

		// customize styling
		plt.Title("Compared plot");
		plt.YAxis.Label("Count (#)");
		plt.XAxis.Label("Time taken (ms)");

		plt.Legend(location: Alignment.UpperLeft);
		plt.SetAxisLimits(yMin: 0);

		return null;
	}

	private static bool CanPlot(Request? r)
	{
		return r != null && r.AttackResult != null && r.AttackOptions != null;
	}
}
