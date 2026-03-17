using Abstractions.OperationHistory;
using Abstractions.Queries;
using Abstractions.Repositories;
using Abstractions.Transactions;
using Contracts.Accounts;
using Contracts.Accounts.Operations;
using Lab1.Application.Mappers;
using Lab1.Domain.Accounts;
using Lab1.Domain.Operations;
using Lab1.Domain.Sessions;
using Lab1.Domain.ValueObjects;
using System.Data;
using System.Diagnostics;

namespace Lab1.Application.Services;

public sealed class AccountService : IAccountService
{
    // TODO: Add optional for it
    private const IsolationLevel TransactionIsolationLevel = IsolationLevel.ReadCommitted;

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
        IOperationHistoryWriter operationWriter)
    {
        _accountRepository = accountRepository;
        _adminSessionRepository = adminSessionRepository;
        _userSessionRepository = userSessionRepository;
        _transactionProvider = transactionProvider;
        _operationWriter = operationWriter;
    }

    public async Task<CreateAccount.Response> CreateAccountAsync(
        CreateAccount.Request request,
        CancellationToken cancellationToken)
    {
        var requestSession = new SessionId(request.SessionId);
        var pinCode = new PinCode(request.PinCode);

        if (await FindAdminSessionById(requestSession, cancellationToken) is null)
        {
            return new CreateAccount.Response.Failure("Session not found");
        }

        await using ITransaction transaction =
            await _transactionProvider.BeginTransactionAsync(cancellationToken, TransactionIsolationLevel);

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

        UserSession? foundSession = await _userSessionRepository
            .QueryAsync(
                SessionQuery.Build(builder => builder.WithPageSize(1).WithSessionId(requestSession)),
                cancellationToken)
            .FirstOrDefaultAsync(cancellationToken);

        if (foundSession is null)
        {
            return new CheckBalance.Response.Failure("Session not found");
        }

        Account account = await FindAccountById(foundSession.AccountId, cancellationToken)
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

        await using ITransaction transaction =
            await _transactionProvider.BeginTransactionAsync(cancellationToken, TransactionIsolationLevel);

        UserSession? foundSession =
            await _userSessionRepository.FindBySessionIdAsync(requestSession, cancellationToken);
        if (foundSession is null)
        {
            return new WithdrawMoney.Response.Failure("Session not found");
        }

        Account account = await FindAccountById(foundSession.AccountId, cancellationToken)
                          ?? throw new UnreachableException("session not bound to account");
        var newAccount = new Account(account.Id, account.PinCode, account.Balance.DecreaseBy(requestMoney));
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

        await using ITransaction transaction =
            await _transactionProvider.BeginTransactionAsync(cancellationToken, TransactionIsolationLevel);

        UserSession? foundSession =
            await _userSessionRepository.FindBySessionIdAsync(requestSession, cancellationToken);
        if (foundSession is null)
        {
            return new DepositMoney.Response.Failure("Session not found");
        }

        Account account = await FindAccountById(foundSession.AccountId, cancellationToken)
                          ?? throw new UnreachableException("session not bound to account");
        var newAccount = new Account(account.Id, account.PinCode, account.Balance.IncreaseBy(requestMoney));
        newAccount = await _accountRepository.UpdateAsync(newAccount, cancellationToken);

        await _operationWriter.AddOperationRecordAsync(
            OperationType.DepositMoney, account.Id, requestSession, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return new DepositMoney.Response.Success(newAccount.MapToDto());
    }

    /*public async Task<GetOperationHistory.Response> OperationHistoryAsync(
        GetOperationHistory.Request request,
        CancellationToken cancellationToken)
    {
        UserSession? foundSession = await _userSessionRepository
            .FindBySessionIdAsync(new SessionId(request.SessionId), cancellationToken);
        if (foundSession is null)
        {
            return new GetOperationHistory.Response.Failure("Session not found");
        }

        OperationRecordId? inputKeyCursor =
            request.PageToken is null ? null : new OperationRecordId(request.PageToken.Token);

        OperationRecord[] operations = await _operationRepository.QueryAsync(
            OperationQuery.Build(builder => builder
                .WithAccountId(foundSession.AccountId)
                .WithKeyCursor(inputKeyCursor)
                .WithPageSize(request.PageSize)),
            cancellationToken).ToArrayAsync(cancellationToken);

        long? keyCursor = operations.Length > 0 ? operations[^1].Id.Value : null;
        return new GetOperationHistory.Response.Success(
            new HistoryDto(
                operations.Select(record => record.MapToDto()).ToList()),
            keyCursor);
    }*/

    private async Task<Account?> FindAccountById(AccountId accountId, CancellationToken cancellationToken)
    {
        return await _accountRepository
            .QueryAsync(
                AccountQuery.Build(builder =>
                    builder.WithPageSize(1).WithAccountId(accountId)),
                cancellationToken)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<AdminSession?> FindAdminSessionById(SessionId sessionId, CancellationToken cancellationToken)
    {
        return await _adminSessionRepository
            .QueryAsync(
                SessionQuery.Build(builder => builder.WithPageSize(1).WithSessionId(sessionId)),
                cancellationToken)
            .FirstOrDefaultAsync(cancellationToken);
    }
}