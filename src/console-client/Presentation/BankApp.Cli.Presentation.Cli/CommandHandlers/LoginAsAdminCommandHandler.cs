using BankApp.Cli.Application.Contracts.Operations;
using BankApp.Cli.Application.Contracts.Services;
using BankApp.Cli.Presentation.Cli.Commands;
using BankApp.Cli.Presentation.Cli.Model;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;

namespace BankApp.Cli.Presentation.Cli.CommandHandlers;

public class LoginAsAdminCommandHandler : AsyncCommand<LoginAsAdminCommand>
{
    private readonly ISessionService _sessionService;
    private readonly ConsolePrinter _consolePrinter;

    public LoginAsAdminCommandHandler(ISessionService sessionService, ConsolePrinter consolePrinter)
    {
        _sessionService = sessionService;
        _consolePrinter = consolePrinter;
    }

    public override async Task<int> ExecuteAsync(
        CommandContext context,
        LoginAsAdminCommand settings,
        CancellationToken cancellationToken)
    {
        string? systemPassword = settings.SystemPassword;
        if (systemPassword is null)
        {
            systemPassword = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter system password: ")
                    .Secret(null));
        }

        var request = new CreateAdminSession.Request(systemPassword, true);

        CreateAdminSession.Result response = await _sessionService.CreateAdminSessionAsync(request, cancellationToken);

        if (response is CreateAdminSession.Result.Success success)
        {
            AnsiConsole.MarkupLine("[green]Success![/]");
            return 0;
        }

        if (response is CreateAdminSession.Result.Failure failure)
        {
            _consolePrinter.PrintFailure(failure.Reason);
            return 1;
        }

        throw new UnreachableException();
    }
}