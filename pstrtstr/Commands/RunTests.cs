using System.Diagnostics.CodeAnalysis;
using PosterLib.Domain;
using Spectre.Console;
using Spectre.Console.Cli;

namespace pstrtstr.Commands;


public class RunTestsSettings : CommandSettings
{
	[CommandArgument(0, "[GROUP]")]
	public string GroupFile { get; set; } = string.Empty;
}


public class RunTestsCommand : Command<RunTestsSettings>
{
	public override int Execute([NotNull] CommandContext context, [NotNull] RunTestsSettings settings)
	{
		try
		{
			var loaded = Disk.LoadRequests(settings.GroupFile);
			AnsiConsole.MarkupLineInterpolated($"Loaded file with gui [red]{loaded.Guid}[/]");

			// todo(Gustav): actually do something with the group...

			return 0;
		}
		catch(System.IO.FileNotFoundException x)
		{
			AnsiConsole.MarkupLineInterpolated($"Could not open [red]{x.FileName}[/]");
			return 1;
		}
	}
}


