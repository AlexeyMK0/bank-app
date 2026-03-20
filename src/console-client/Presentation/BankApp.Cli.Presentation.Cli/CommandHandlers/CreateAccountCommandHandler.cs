using BankApp.Cli.Application.Contracts.Operations;
using BankApp.Cli.Application.Contracts.Services;
using BankApp.Cli.Presentation.Cli.Commands;
using BankApp.Cli.Presentation.Cli.Model;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;

namespace BankApp.Cli.Presentation.Cli.CommandHandlers;

public class CreateAccountCommandHandler : AsyncCommand<CreateAccountCommand>
{
    private readonly IAccountService _accountService;
    private readonly ConsolePrinter _consolePrinter;

    public CreateAccountCommandHandler(IAccountService accountService, ConsolePrinter consolePrinter)
    {
        _accountService = accountService;
        _consolePrinter = consolePrinter;
    }

    public override async Task<int> ExecuteAsync(
        CommandContext context,
        CreateAccountCommand settings,
        CancellationToken cancellationToken)
    {
        var request = new CreateAccount.Request(settings.PinCode);

        CreateAccount.Result response = await _accountService.CreateAccountAsync(request, cancellationToken);

        if (response is CreateAccount.Result.Success success)
        {
            AnsiConsole.MarkupLine("[green]Success![/]");
            _consolePrinter.PrintAccount(success.Account);
            return 0;
        }

        if (response is CreateAccount.Result.Failure failure)
        {
            _consolePrinter.PrintFailure(failure.Reason);
            return 1;
        }

        throw new UnreachableException();
    }
}