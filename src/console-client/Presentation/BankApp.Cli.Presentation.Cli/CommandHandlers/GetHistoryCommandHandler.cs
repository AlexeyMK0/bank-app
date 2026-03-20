using BankApp.Cli.Application.Contracts.Model;
using BankApp.Cli.Application.Contracts.Operations;
using BankApp.Cli.Application.Contracts.Services;
using BankApp.Cli.Presentation.Cli.Commands;
using BankApp.Cli.Presentation.Cli.Model;
using BankApp.Cli.Presentation.Cli.Options;
using Microsoft.Extensions.Options;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;

namespace BankApp.Cli.Presentation.Cli.CommandHandlers;

public class GetHistoryCommandHandler : AsyncCommand<GetHistoryCommand>
{
    private readonly int _defaultEntriesCount;
    private readonly IAccountService _accountService;
    private readonly ConsolePrinter _consolePrinter;

    public GetHistoryCommandHandler(IAccountService accountService, ConsolePrinter consolePrinter, IOptions<CliOptions> cliOptions)
    {
        _accountService = accountService;
        _consolePrinter = consolePrinter;
        _defaultEntriesCount = cliOptions.Value.DefaultEntriesCount;
    }

    public override async Task<int> ExecuteAsync(
        CommandContext context,
        GetHistoryCommand settings,
        CancellationToken cancellationToken)
    {
        var request = new GetHistory.Request(settings.EntriesCount ?? _defaultEntriesCount);

        GetHistory.Result response = await _accountService.GetHistoryAsync(request, cancellationToken);

        if (response is GetHistory.Result.Success success)
        {
            PrintOperations(success.Operations);
            return 0;
        }

        if (response is GetHistory.Result.Failure failure)
        {
            _consolePrinter.PrintFailure(failure.Reason);
            return 1;
        }

        throw new UnreachableException();
    }

    private void PrintOperations(IEnumerable<OperationRecordDto> operations)
    {
        Table table = new Table()
            .AddColumn("Time")
            .AddColumn("Type");

        foreach (OperationRecordDto operation in operations)
        {
            table.AddRow(operation.Time.ToString(), operation.OperationType);
        }

        AnsiConsole.Write(table);
    }
}