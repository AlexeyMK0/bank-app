using BankApp.Cli.Application.Contracts.Operations;
using BankApp.Cli.Application.Contracts.Services;
using BankApp.Cli.Presentation.Cli.Commands;
using BankApp.Cli.Presentation.Cli.Model;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;

namespace BankApp.Cli.Presentation.Cli.CommandHandlers;

public class DepositMoneyCommandHandler : AsyncCommand<DepositMoneyCommand>
{
    private readonly IAccountService _accountService;
    private readonly ConsolePrinter _consolePrinter;

    public DepositMoneyCommandHandler(IAccountService accountService, ConsolePrinter consolePrinter)
    {
        _accountService = accountService;
        _consolePrinter = consolePrinter;
    }

    public override async Task<int> ExecuteAsync(
        CommandContext context,
        DepositMoneyCommand settings,
        CancellationToken cancellationToken)
    {
        var request = new DepositMoney.Request(settings.Amount);

        DepositMoney.Result response = await _accountService.DepositMoneyAsync(request, cancellationToken);

        if (response is DepositMoney.Result.Success success)
        {
            AnsiConsole.MarkupLine("[green]Success![/]");
            AnsiConsole.MarkupLine($"Updated balance: {success.UpdatedBalance}");
            return 0;
        }

        if (response is DepositMoney.Result.Failure failure)
        {
            _consolePrinter.PrintFailure(failure.Reason);
            return 1;
        }

        throw new UnreachableException();
    }
}