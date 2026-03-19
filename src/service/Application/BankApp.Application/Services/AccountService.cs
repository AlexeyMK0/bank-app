using Abstractions.OperationHistory;
using Abstractions.Repositories;
using Abstractions.Transactions;
using Contracts.Accounts;
using Contracts.Accounts.Operations;
using Lab1.Application.Mappers;
using Lab1.Application.Model;
using Lab1.Application.RepositoryExtensions;
using Lab1.Domain.Accounts;
using Lab1.Domain.Operations;
using Lab1.Domain.Sessions;
using Lab1.Domain.ValueObjects;
using Microsoft.Extensions.Options;
using System.Data;
using System.Diagnostics;

namespace Lab1.Application.Services;

public sealed class AccountService : IAccountService
{
    private readonly IsolationLevel _isolationLevel;

    private readonly IAccountRepository _accountRepository;
    private readonly IAdminSessionRepository _adminSessionRepository;
    private readonly IUserSessionRepository _userSessionRepository;
    private readonly ITransactionProvider _transactionProvider;
    private readonly IOperationHistoryWriter _operationWriter;

    public AccountService(
        IAccountRepository accountRepository,
        IAdminSessionRepository adminSessionRepository,
        IUserSessionRepository userSessionRepository,
        ITransactionProvider transactionProvider,
        IOperationHistoryWriter operationWriter,
        IOptions<DefaultIsolationLevel> isolationLevel)
    {
        _accountRepository = accountRepository;
        _adminSessionRepository = adminSessionRepository;
        _userSessionRepository = userSessionRepository;
        _transactionProvider = transactionProvider;
        _operationWriter = operationWriter;
        _isolationLevel = isolationLevel.Value.IsolationLevel;
    }

    public async Task<CreateAccount.Response> CreateAccountAsync(
        CreateAccount.Request request,
        CancellationToken cancellationToken)
    {
        var requestSession = new SessionId(request.SessionId);
        var pinCode = new PinCode(request.PinCode);

        if (await _adminSessionRepository.FindAdminSessionById(requestSession, cancellationToken) is null)
        {
            return new CreateAccount.Response.Failure("Session not found");
        }

        await using ITransaction transaction =
            await _transactionProvider.BeginTransactionAsync(cancellationToken, _isolationLevel);

        Account account = await _accountRepository.AddAsync(
            new Account(AccountId.Default, pinCode, Money.Zero),
            cancellationToken);

        await _operationWriter.AddOperationRecordAsync(
            OperationType.CreateAccount, account.Id, requestSession, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return new CreateAccount.Response.Success(account.MapToDto());
    }

    public async Task<CheckBalance.Response> CheckBalanceAsync(
        CheckBalance.Request request,
        CancellationToken cancellationToken)
    {
        var requestSession = new SessionId(request.SessionId);

        UserSession? foundSession
            = await _userSessionRepository.FindBySessionIdAsync(requestSession, cancellationToken);

        if (foundSession is null)
        {
            return new CheckBalance.Response.Failure("Session not found");
        }

        Account account = await _accountRepository.FindAccountById(foundSession.AccountId, cancellationToken)
            ?? throw new UnreachableException("session not bound to account");

        await _operationWriter.AddOperationRecordAsync(
            OperationType.CheckBalance, account.Id, requestSession, cancellationToken);

        return new CheckBalance.Response.Success(account.Balance.Value);
    }

    public async Task<WithdrawMoney.Response> WithdrawMoneyAsync(
        WithdrawMoney.Request request,
        CancellationToken cancellationToken)
    {
        var requestSession = new SessionId(request.SessionId);
        var requestMoney = new Money(request.Amount);
        UserSession? foundSession =
            await _userSessionRepository.FindBySessionIdAsync(requestSession, cancellationToken);
        if (foundSession is null)
        {
            return new WithdrawMoney.Response.Failure("Session not found");
        }

        Account account = await _accountRepository.FindAccountById(foundSession.AccountId, cancellationToken)
                          ?? throw new UnreachableException("session not bound to account");
        var newAccount = new Account(account.Id, account.PinCode, account.Balance.DecreaseBy(requestMoney));

        await using ITransaction transaction =
            await _transactionProvider.BeginTransactionAsync(cancellationToken, _isolationLevel);

        newAccount = await _accountRepository.UpdateAsync(newAccount, cancellationToken);
        await _operationWriter.AddOperationRecordAsync(
            OperationType.WithdrawMoney, account.Id, requestSession, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return new WithdrawMoney.Response.Success(newAccount.MapToDto());
    }

    public async Task<DepositMoney.Response> DepositMoneyAsync(
        DepositMoney.Request request,
        CancellationToken cancellationToken)
    {
        var requestSession = new SessionId(request.SessionId);
        var requestMoney = new Money(request.Amount);

        UserSession? foundSession =
            await _userSessionRepository.FindBySessionIdAsync(requestSession, cancellationToken);
        if (foundSession is null)
        {
            return new DepositMoney.Response.Failure("Session not found");
        }

        Account account = await _accountRepository.FindAccountById(foundSession.AccountId, cancellationToken)
                          ?? throw new UnreachableException("session not bound to account");
        var newAccount = new Account(account.Id, account.PinCode, account.Balance.IncreaseBy(requestMoney));

        await using ITransaction transaction =
            await _transactionProvider.BeginTransactionAsync(cancellationToken, _isolationLevel);

        newAccount = await _accountRepository.UpdateAsync(newAccount, cancellationToken);
        await _operationWriter.AddOperationRecordAsync(
            OperationType.DepositMoney, account.Id, requestSession, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return new DepositMoney.Response.Success(newAccount.MapToDto());
    }

    /*private async Task<Account?> FindAccountById(AccountId accountId, CancellationToken cancellationToken)
    {
        return await _accountRepository
            .QueryAsync(
                AccountQuery.Build(builder =>
                    builder.WithPageSize(1).WithAccountId(accountId)),
                cancellationToken)
            .FirstOrDefaultAsync(cancellationToken);
    }*/

    /*private async Task<AdminSession?> FindAdminSessionById(SessionId sessionId, CancellationToken cancellationToken)
    {
        return await _adminSessionRepository
            .QueryAsync(
                SessionQuery.Build(builder => builder.WithPageSize(1).WithSessionId(sessionId)),
                cancellationToken)
            .FirstOrDefaultAsync(cancellationToken);
    }*/
}