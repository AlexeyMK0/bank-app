using Spectre.Console.Cli;

namespace BankApp.Cli.Presentation.Cli.Commands;

public class GetHistoryCommand : CommandSettings
{
    [CommandOption("-c|--count")]
    public int? EntriesCount { get; init; }
}