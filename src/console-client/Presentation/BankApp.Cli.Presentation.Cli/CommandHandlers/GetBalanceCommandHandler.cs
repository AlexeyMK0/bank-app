using BankApp.Cli.Application.Contracts.Operations;
using BankApp.Cli.Application.Contracts.Services;
using BankApp.Cli.Presentation.Cli.Commands;
using BankApp.Cli.Presentation.Cli.Model;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;

namespace BankApp.Cli.Presentation.Cli.CommandHandlers;

public class GetBalanceCommandHandler : AsyncCommand<GetBalanceCommand>
{
    private readonly IAccountService _accountService;
    private readonly ConsolePrinter _consolePrinter;

    public GetBalanceCommandHandler(IAccountService accountService, ConsolePrinter consolePrinter)
    {
        _accountService = accountService;
        _consolePrinter = consolePrinter;
    }

    public override async Task<int> ExecuteAsync(
        CommandContext context,
        GetBalanceCommand settings,
        CancellationToken cancellationToken)
    {
        var request = new GetBalance.Request();

        GetBalance.Result response = await _accountService.GetBalanceAsync(request, cancellationToken);

        if (response is GetBalance.Result.Success success)
        {
            AnsiConsole.MarkupLine("[green]Success![/]");
            AnsiConsole.MarkupLine($"Balance: {success.Balance}");
            return 0;
        }

        if (response is GetBalance.Result.Failure failure)
        {
            _consolePrinter.PrintFailure(failure.Reason);
            return 1;
        }

        throw new UnreachableException();
    }
}