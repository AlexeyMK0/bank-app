using BankApp.Cli.Application.Contracts.Operations;
using BankApp.Cli.Application.Contracts.Services;
using BankApp.Cli.Presentation.Cli.Commands;
using BankApp.Cli.Presentation.Cli.Model;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;

namespace BankApp.Cli.Presentation.Cli.CommandHandlers;

public class WithdrawMoneyCommandHandler : AsyncCommand<WithdrawMoneyCommand>
{
    private readonly IAccountService _accountService;
    private readonly ConsolePrinter _consolePrinter;

    public WithdrawMoneyCommandHandler(IAccountService accountService, ConsolePrinter consolePrinter)
    {
        _accountService = accountService;
        _consolePrinter = consolePrinter;
    }

    public override async Task<int> ExecuteAsync(
        CommandContext context,
        WithdrawMoneyCommand settings,
        CancellationToken cancellationToken)
    {
        var request = new WithdrawMoney.Request(settings.Amount);

        WithdrawMoney.Result response = await _accountService.WithdrawMoneyAsync(request, cancellationToken);

        if (response is WithdrawMoney.Result.Success success)
        {
            AnsiConsole.MarkupLine("[green]Success![/]");
            AnsiConsole.MarkupLine($"Updated balance: {success.UpdatedBalance}");
            return 0;
        }

        if (response is WithdrawMoney.Result.Failure failure)
        {
            _consolePrinter.PrintFailure(failure.Reason);
            return 1;
        }

        throw new UnreachableException();
    }
}