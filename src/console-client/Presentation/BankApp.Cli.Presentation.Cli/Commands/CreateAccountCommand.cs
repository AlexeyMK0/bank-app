using Spectre.Console.Cli;

namespace BankApp.Cli.Presentation.Cli.Commands;

public class CreateAccountCommand : CommandSettings
{
    [CommandOption("-p|--pin")]
    public required string PinCode { get; init; }
}