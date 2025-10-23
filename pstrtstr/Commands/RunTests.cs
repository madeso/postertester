using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using PosterLib.Data;
using PosterLib.Domain;
using PosterLib.Saved;
using Spectre.Console;
using Spectre.Console.Cli;

namespace pstrtstr.Commands;


public class RunTestsSettings : CommandSettings
{
	[CommandArgument(0, "[GROUP]")]
	[Description("The group/service file (*.PosterTesterGroup)")]
	public string GroupFile { get; set; } = string.Empty;

	[CommandOption("-t|--bearer-token <TOKEN>")]
	[Description("The bearer token to use for requests that require authentication")]
	public string? BearerToken { get; set; } = null;

	[CommandOption("-b|--base-url <URL>")]
	[Description("The base url to use for requests that use the base url")]
	public string? BaseUrl { get; set; } = null;

	[CommandOption("-p|--passed")]
	[Description("Write passed tests to the report")]
	public bool WritePassedTests { get; set; } = false;
}


public class RunTestsCommand : AsyncCommand<RunTestsSettings>
{
	public override Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] RunTestsSettings settings)
	{
		return run(context, settings);
	}

	private static async Task<int> run(CommandContext context, RunTestsSettings settings)
	{
		int errorCount = 0;

		var okTests = new List<string>();
		var failedTests = new List<string>();

		try
		{
			var loaded = Disk.LoadSharedGroupFile(settings.GroupFile);
			AnsiConsole.MarkupLineInterpolated($"Loaded file with gui [red]{loaded.Guid}[/]");

			if (settings.BaseUrl == null && loaded.UseBaseUrl)
			{
				AnsiConsole.MarkupLineInterpolated($"ERROR: Missing required base url!");
				return -1;
			}

			var auth = new AuthData { BearerToken = settings.BearerToken ?? string.Empty };

			var group = new GroupSettings(loaded.UseBaseUrl, settings.BaseUrl ?? "");

			// Synchronous
			await AnsiConsole.Status()
				.StartAsync(CreateStatus(errorCount), async ctx =>
				{
					using var cancel = new CancellationTokenSource();
					foreach (var req in loaded.Requests)
					{
						var testName = req.TitleOrUrl;
						AnsiConsole.MarkupLineInterpolated($"Running [blue]{testName}[/]");
						var response = await Logic.SingleAttack(group, req, auth, cancel.Token);
						
						if (response.Error != null)
						{
							errorCount += 1;
							ctx.Status(CreateStatus(errorCount));
							ctx.SpinnerStyle(Style.Parse("red"));
						
							AnsiConsole.MarkupLine("Test [red]FAILED[/]");
							AnsiConsole.MarkupInterpolated($"[red]{response.Error}[/]");
							failedTests.Add(testName);
						}
						else
						{
							var timeTaken = Logic.TimeSpanToString(response.Result);
							AnsiConsole.MarkupLine("Test [green]OK[/]");
							AnsiConsole.MarkupInterpolated($"Took [blue]{timeTaken}[/]");
							okTests.Add(testName);
						}
					}
				});

			// Print report
			int totalTests = loaded.Requests.Count;
			AnsiConsole.WriteLine();
			AnsiConsole.MarkupLine("[underline]Test Report[/]");
			foreach (string test in failedTests)
			{
				AnsiConsole.MarkupLineInterpolated($"* [red]{test}[/]");
			}
			AnsiConsole.MarkupLineInterpolated($"[red]{failedTests.Count}[/]/{totalTests} tests [red]FAILED[/] ({PercentageString(failedTests.Count / (float)totalTests)}).");
			if(settings.WritePassedTests)
			{
				foreach (string test in okTests)
				{
					AnsiConsole.MarkupLineInterpolated($"* [green]{test}[/]");
				}
			}
			AnsiConsole.MarkupLineInterpolated($"[green]{okTests.Count}[/]/{totalTests} tests passed ({PercentageString(okTests.Count / (float)totalTests)}).");

			if (errorCount > 0)
			{
				return -errorCount;
			}
			else
			{
				return 0;
			}
		}
		catch (System.IO.FileNotFoundException x)
		{
			AnsiConsole.MarkupLineInterpolated($"Could not open [red]{x.FileName}[/]");
			return 1;
		}
	}

	private static string PercentageString(float percentage)
	{
		return (percentage * 100).ToString("0.00") + "%";
	}

	private static void SetSpinnerStatus(Logic.SingleAttackResult res, StatusContext ctx, ref int errorCount)
	{
		if (res.Error != null)
		{
			errorCount += 1;
			ctx.Status(CreateStatus(errorCount));
			ctx.SpinnerStyle(Style.Parse("red"));
		}
	}

	private static string CreateStatus(int errorCount)
	{
		if(errorCount == 0)
		{
			return "Working...";
		}
		else
		{ 
			return $"Working... ({errorCount} errors detected)";
		}
	}
}


