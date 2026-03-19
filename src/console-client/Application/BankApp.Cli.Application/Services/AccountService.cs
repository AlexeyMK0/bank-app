using BankApp.Cli.Application.Abstractions.Clients;
using BankApp.Cli.Application.Abstractions.Operations;
using BankApp.Cli.Application.Contracts.Model;
using BankApp.Cli.Application.Contracts.Operations;
using BankApp.Cli.Application.Contracts.Services;
using BankApp.Cli.Application.Mappers;
using BankApp.Cli.Application.Model;
using System.Diagnostics;

namespace BankApp.Cli.Application.Services;

// TODO: add service client extension
public class AccountService : IAccountService
{
    // TODO: add optional
    private const int DefaultEntriesCount = 10;
    private readonly ISessionKeeper _sessionKeeper;
    private readonly IAccountClient _accountClient;

    public AccountService(ISessionKeeper sessionKeeper, IAccountClient accountClient)
    {
        _sessionKeeper = sessionKeeper;
        _accountClient = accountClient;
    }

    public async Task<CreateAccount.Result> CreateAccountAsync(CreateAccount.Request request, CancellationToken cancellationToken)
    {
        Guid? currentSession = _sessionKeeper.CurrentSession;
        if (currentSession is null)
        {
            return new CreateAccount.Result.Failure("You need to log in to perform any operation");
        }

        CreateAccountClient.Result result = await _accountClient.CreateNewAccountAsync(
            new CreateAccountClient.Request(request.PinCode, currentSession),
            cancellationToken);

        return result switch
        {
            CreateAccountClient.Result.Success success => new CreateAccount.Result.Success(
                new AccountDto(success.CreatedAccount.Id, success.CreatedAccount.Balance)),
            CreateAccountClient.Result.Failure failure => new CreateAccount.Result.Failure(failure.Reason),
            _ => throw new UnreachableException(),
        };
    }

    public async Task<GetBalance.Result> GetBalanceAsync(GetBalance.Request request, CancellationToken cancellationToken)
    {
        Guid? currentSession = _sessionKeeper.CurrentSession;
        if (currentSession is null)
        {
            return new GetBalance.Result.Failure("You need to log in to perform any operation");
        }

        GetBalanceClient.Result result = await _accountClient.GetBalanceAsync(
            new GetBalanceClient.Request(currentSession),
            cancellationToken);

        return result switch
        {
            GetBalanceClient.Result.Success success => new GetBalance.Result.Success(success.Balance),
            GetBalanceClient.Result.Failure failure => new GetBalance.Result.Failure(failure.Reason),
            _ => throw new UnreachableException(),
        };
    }

    public async Task<WithdrawMoney.Result> WithdrawMoneyAsync(WithdrawMoney.Request request, CancellationToken cancellationToken)
    {
        Guid? currentSession = _sessionKeeper.CurrentSession;
        if (currentSession is null)
        {
            return new WithdrawMoney.Result.Failure("You need to log in to perform any operation");
        }

        WithdrawMoneyClient.Result result = await _accountClient.WithdrawMoneyAsync(
            new WithdrawMoneyClient.Request(request.Amount, currentSession),
            cancellationToken);

        return result switch
        {
            WithdrawMoneyClient.Result.Success success => new WithdrawMoney.Result.Success(success.UpdatedBalance),
            WithdrawMoneyClient.Result.Failure failure => new WithdrawMoney.Result.Failure(failure.Reason),
            _ => throw new UnreachableException(),
        };
    }

    public async Task<DepositMoney.Result> DepositMoneyAsync(DepositMoney.Request request, CancellationToken cancellationToken)
    {
        Guid? currentSession = _sessionKeeper.CurrentSession;
        if (currentSession is null)
        {
            return new DepositMoney.Result.Failure("You need to log in to perform any operation");
        }

        DepositMoneyClient.Result result = await _accountClient.DepositMoneyAsync(
            new DepositMoneyClient.Request(request.Amount, currentSession),
            cancellationToken);

        return result switch
        {
            DepositMoneyClient.Result.Success success => new DepositMoney.Result.Success(success.UpdatedBalance),
            DepositMoneyClient.Result.Failure failure => new DepositMoney.Result.Failure(failure.Reason),
            _ => throw new UnreachableException(),
        };
    }

    public async Task<GetHistory.Result> GetHistoryAsync(GetHistory.Request request, CancellationToken cancellationToken)
    {
        Guid? currentSession = _sessionKeeper.CurrentSession;
        if (currentSession is null)
        {
            return new GetHistory.Result.Failure("You need to log in to perform any operation");
        }

        GetHistoryClient.Result result = await _accountClient.GetHistoryAsync(
            new GetHistoryClient.Request(currentSession, request.EntriesCount ?? DefaultEntriesCount),
            cancellationToken);

        return result switch
        {
            GetHistoryClient.Result.Success success => new GetHistory.Result.Success(
                success.Entries.Select(entry => entry.MapToDto())),
            GetHistoryClient.Result.Failure failure => new GetHistory.Result.Failure(failure.Reason),
            _ => throw new UnreachableException(),
        };
    }
}