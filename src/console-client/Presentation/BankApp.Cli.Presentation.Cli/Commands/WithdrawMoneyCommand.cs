using Spectre.Console.Cli;

namespace BankApp.Cli.Presentation.Cli.Commands;

public class WithdrawMoneyCommand : CommandSettings
{
    [CommandArgument(0, "<AMOUNT>")]
    public required decimal Amount { get; set; }
}