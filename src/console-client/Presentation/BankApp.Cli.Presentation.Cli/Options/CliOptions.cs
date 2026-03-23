using System.ComponentModel.DataAnnotations;

namespace BankApp.Cli.Presentation.Cli.Options;

public class CliOptions
{
    [Range(1, int.MaxValue)]
    public int DefaultEntriesCount { get; init; }
}