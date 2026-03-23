using BankApp.Cli.Application.Abstractions.Clients;
using BankApp.Cli.Application.Abstractions.Operations;
using BankApp.Cli.Application.Contracts.Model;
using BankApp.Cli.Application.Contracts.Operations;
using BankApp.Cli.Application.Contracts.Services;
using BankApp.Cli.Application.Mappers;
using BankApp.Cli.Application.Model;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BankApp.Cli.Application.Services;

public class AccountService : IAccountService
{
    private readonly ILogger<AccountService> _logger;

    private readonly IUserContext _userContext;
    private readonly IAccountClient _accountClient;

    public AccountService(IUserContext userContext, IAccountClient accountClient, ILogger<AccountService> logger)
    {
        _userContext = userContext;
        _accountClient = accountClient;
        _logger = logger;
    }

    public async Task<CreateAccount.Result> CreateAccountAsync(CreateAccount.Request request, CancellationToken cancellationToken)
    {
        Guid? currentSession = _userContext.CurrentSession;
        if (currentSession is null)
        {
            return new CreateAccount.Result.Failure("You need to log in to perform any operation");
        }

        CreateAccountRequest.Result result = await _accountClient.CreateNewAccountAsync(
            new CreateAccountRequest.Request(request.PinCode, (Guid)currentSession),
            cancellationToken);

        return result switch
        {
            CreateAccountRequest.Result.Success success => new CreateAccount.Result.Success(
                new AccountDto(success.CreatedAccount.Id, success.CreatedAccount.Balance)),
            CreateAccountRequest.Result.Failure failure => new CreateAccount.Result.Failure(failure.Reason),
            _ => throw new UnreachableException(),
        };
    }

    public async Task<GetBalance.Result> GetBalanceAsync(GetBalance.Request request, CancellationToken cancellationToken)
    {
        Guid? currentSession = _userContext.CurrentSession;
        if (currentSession is null)
        {
            return new GetBalance.Result.Failure("You need to log in to perform any operation");
        }

        GetBalanceRequest.Result result = await _accountClient.GetBalanceAsync(
            new GetBalanceRequest.Request((Guid)currentSession),
            cancellationToken);

        return result switch
        {
            GetBalanceRequest.Result.Success success => new GetBalance.Result.Success(success.Balance),
            GetBalanceRequest.Result.Failure failure => new GetBalance.Result.Failure(failure.Reason),
            _ => throw new UnreachableException(),
        };
    }

    public async Task<WithdrawMoney.Result> WithdrawMoneyAsync(WithdrawMoney.Request request, CancellationToken cancellationToken)
    {
        Guid? currentSession = _userContext.CurrentSession;
        if (currentSession is null)
        {
            return new WithdrawMoney.Result.Failure("You need to log in to perform any operation");
        }

        WithdrawMoneyRequest.Result result = await _accountClient.WithdrawMoneyAsync(
            new WithdrawMoneyRequest.Request(request.Amount, (Guid)currentSession),
            cancellationToken);

        return result switch
        {
            WithdrawMoneyRequest.Result.Success success => new WithdrawMoney.Result.Success(success.UpdatedBalance),
            WithdrawMoneyRequest.Result.Failure failure => new WithdrawMoney.Result.Failure(failure.Reason),
            _ => throw new UnreachableException(),
        };
    }

    public async Task<DepositMoney.Result> DepositMoneyAsync(DepositMoney.Request request, CancellationToken cancellationToken)
    {
        Guid? currentSession = _userContext.CurrentSession;
        if (currentSession is null)
        {
            return new DepositMoney.Result.Failure("You need to log in to perform any operation");
        }

        DepositMoneyRequest.Result result = await _accountClient.DepositMoneyAsync(
            new DepositMoneyRequest.Request(request.Amount, (Guid)currentSession),
            cancellationToken);

        return result switch
        {
            DepositMoneyRequest.Result.Success success => new DepositMoney.Result.Success(success.UpdatedBalance),
            DepositMoneyRequest.Result.Failure failure => new DepositMoney.Result.Failure(failure.Reason),
            _ => throw new UnreachableException(),
        };
    }

    public async Task<GetHistory.Result> GetHistoryAsync(GetHistory.Request request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetHistory request in service");

        Guid? currentSession = _userContext.CurrentSession;
        if (currentSession is null)
        {
            return new GetHistory.Result.Failure("You need to log in to perform any operation");
        }

        GetHistoryRequest.Result result = await _accountClient.GetHistoryAsync(
            new GetHistoryRequest.Request((Guid)currentSession, request.EntriesCount),
            cancellationToken);

        _logger.LogInformation("Got GetHistory response in service");

        return result switch
        {
            GetHistoryRequest.Result.Success success => new GetHistory.Result.Success(
                success.Entries.Select(entry => entry.MapToDto())),
            GetHistoryRequest.Result.Failure failure => new GetHistory.Result.Failure(failure.Reason),
            _ => throw new UnreachableException(),
        };
    }
}