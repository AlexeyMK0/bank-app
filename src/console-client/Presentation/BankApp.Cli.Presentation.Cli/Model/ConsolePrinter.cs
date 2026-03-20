using BankApp.Cli.Application.Contracts.Model;
using Spectre.Console;

namespace BankApp.Cli.Presentation.Cli.Model;

public class ConsolePrinter
{
    public void PrintFailure(string error)
    {
        AnsiConsole.Markup($"[red]Error occured: '{error}'[/]");
    }

    public void PrintAccount(AccountDto account)
    {
        AnsiConsole.MarkupLine($"AccountInfo:");
        AnsiConsole.MarkupLine($"id: [blue]{account.AccountId}[/]");
        AnsiConsole.MarkupLine($"balance: [blue]{account.Balance}[/]");
    }
}