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
	public string GroupFile { get; set; } = string.Empty;
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
		try
		{
			var loaded = Disk.LoadRequests(settings.GroupFile);
			AnsiConsole.MarkupLineInterpolated($"Loaded file with gui [red]{loaded.Guid}[/]");

			// Synchronous
			await AnsiConsole.Status()
				.StartAsync(CreateStatus(errorCount), async ctx =>
				{
					foreach (var req in loaded.Requests)
					{
						AnsiConsole.MarkupLineInterpolated($"Running [blue]{req.TitleOrUrl}[/]");
						var response = await Logic.SingleAttack(req);
						SetSpinnerStatus(response, ctx, ref errorCount);

						if (response.Error != null)
						{
							AnsiConsole.MarkupLine("Test [red]FAILED[/]");
							AnsiConsole.MarkupInterpolated($"[red]{response.Error}[/]");
						}
						else
						{
							var timeTaken = Logic.TimeSpanToString(response.Result);
							AnsiConsole.MarkupLine("Test [green]OK[/]");
							AnsiConsole.MarkupInterpolated($"Took [blue]{timeTaken}[/]");
						}
					}
				});

			if(errorCount > 0)
			{
				int totalTests = loaded.Requests.Count;

				AnsiConsole.MarkupInterpolated($"Error: [red]{errorCount}[/]/{totalTests} tests [red]FAILED[/]");
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


