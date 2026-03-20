using Spectre.Console.Cli;

namespace BankApp.Cli.Presentation.Cli.Commands;

public class LoginAsUserCommand : CommandSettings
{
    // [CommandOption("-i|--id")]
    [CommandArgument(0, "<ACCOUNT_ID>")]
    public required long AccountId { get; init; }

    // [CommandArgument(1, "<PIN_CODE>")]
    [CommandOption("-p|--pin")]
    public string? PinCode { get; init; }
}