using System.Diagnostics.CodeAnalysis;
using Spectre.Console.Cli;

namespace pstrtstr.Commands;

public class RunTestsCommand : Command<RunTestsSettings>
{
	public override int Execute([NotNull] CommandContext context, [NotNull] RunTestsSettings settings)
	{
		return 0;
	}
}

public class RunTestsSettings : CommandSettings
{
	[CommandArgument(0, "[GROUP]")]
	public string? GroupFile { get; set; }
}

