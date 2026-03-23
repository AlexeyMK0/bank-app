using Spectre.Console.Cli;

namespace BankApp.Cli.Presentation.Cli.Commands;

public class LoginAsAdminCommand : CommandSettings
{
    [CommandOption("-p|--password")]
    public string? SystemPassword { get; init; }
}