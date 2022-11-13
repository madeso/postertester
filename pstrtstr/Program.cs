using pstrtstr.Commands;
using Spectre.Console.Cli;

var app = new CommandApp();

app.Configure(config =>
{
	config.AddCommand<RunTestsCommand>("test");
});

return app.Run(args);

