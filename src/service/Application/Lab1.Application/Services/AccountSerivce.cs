using Abstractions.Queries;
using Abstractions.Repositories;
using Abstractions.Transactions;
using Contracts.Accounts;
using Contracts.Accounts.Model;
using Contracts.Accounts.Operations;
using Lab1.Application.Mappers;
using Lab1.Domain.Accounts;
using Lab1.Domain.Sessions;
using Lab1.Domain.ValueObjects;
using System.Diagnostics;

namespace Lab1.Application.Services;

public sealed class AccountSerivce : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAdminSessionRepository _adminSessionRepository;
    private readonly IUserSessionRepository _userSessionRepository;
    private readonly IOperationRepository _operationRepository;
    private readonly ITransactionProvider _transactionProvider;

    public AccountSerivce(
        IAccountRepository accountRepository,
        IAdminSessionRepository adminSessionRepository,
        IUserSessionRepository userSessionRepository,
        IOperationRepository operationRepository,
        ITransactionProvider transactionProvider)
    {
        _accountRepository = accountRepository;
        _adminSessionRepository = adminSessionRepository;
        _userSessionRepository = userSessionRepository;
        _operationRepository = operationRepository;
        _transactionProvider = transactionProvider;
    }

    public async Task<CreateAccount.Response> CreateAccountAsync(
        CreateAccount.Request request,
        CancellationToken cancellationToken)
    {
        var requestSession = new SessionId(request.SessionId);
        var pinCode = new PinCode(request.PinCode);

        await using ITransaction transaction = await _transactionProvider.BeginTransactionAsync(cancellationToken);

        if (await _adminSessionRepository.FindBySessionAsync(requestSession, cancellationToken) is null)
        {
            return new CreateAccount.Response.Failure("Session not found");
        }

        Account account = await _accountRepository.AddAsync(
            new Account(AccountId.Default, new PinCode(request.PinCode), Money.Zero),
            cancellationToken);

        await _transactionProvider.SaveChangesAsync(cancellationToken);
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

        Account? account = await _accountRepository
           .QueryAsync(
               AccountQuery.Build(builder =>
                   builder.WithPageSize(1).WithAccountId(foundSession.AccountId)),
               cancellationToken)
           .FirstOrDefaultAsync(cancellationToken)
             ?? throw new UnreachableException("session not bound to account");

        return new CheckBalance.Response.Success(account.Balance.Value);
    }

    public async Task<WithdrawMoney.Response> WithdrawMoneyAsync(
        WithdrawMoney.Request request,
        CancellationToken cancellationToken)
    {
        var requestSession = new SessionId(request.SessionId);
        var requestMoney = new Money(request.Amount);

        await using ITransaction transaction = await _transactionProvider.BeginTransactionAsync(cancellationToken);

        UserSession? foundSession = await FindUserSessionById(requestSession, cancellationToken);
        if (foundSession is null)
        {
            return new WithdrawMoney.Response.Failure("Session not found");
        }

        Account account = await FindAccountById(foundSession.AccountId, cancellationToken)
            ?? throw new UnreachableException("session not bound to account");
        var newAccount = new Account(account.Id, account.PinCode, account.Balance.DecreaseBy(requestMoney));
        newAccount = await _accountRepository.UpdateAsync(newAccount, cancellationToken);

        await _transactionProvider.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new WithdrawMoney.Response.Success(newAccount.MapToDto());
    }

    public async Task<DepositMoney.Response> DepositMoneyAsync(
        DepositMoney.Request request,
        CancellationToken cancellationToken)
    {
        var requestSession = new SessionId(request.SessionId);
        var requestMoney = new Money(request.Amount);

        await using ITransaction transaction = await _transactionProvider.BeginTransactionAsync(cancellationToken);

        UserSession? foundSession = await FindUserSessionById(requestSession, cancellationToken);
        if (foundSession is null)
        {
            return new DepositMoney.Response.Failure("Session not found");
        }

        Account account = await FindAccountById(foundSession.AccountId, cancellationToken)
            ?? throw new UnreachableException("session not bound to account");
        var newAccount = new Account(account.Id, account.PinCode, account.Balance.IncreaseBy(requestMoney));
        newAccount = await _accountRepository.UpdateAsync(newAccount, cancellationToken);

        await _transactionProvider.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new DepositMoney.Response.Success(newAccount.MapToDto());
    }

    public async Task<OperationHistory.Response> OperationHistoryAsync(
        OperationHistory.Request request,
        CancellationToken cancellationToken)
    {
        UserSession? foundSession = await FindUserSessionById(new SessionId(request.SessionId), cancellationToken);
        if (foundSession is null)
        {
            return new OperationHistory.Response.Failure("Session not found");
        }

        OperationRecord[] operations = await _operationRepository.QueryAsync(
            OperationQuery.Build(builder => builder
                .WithAccountId(foundSession.AccountId)
                .WithKeyCursor(request.KeyCursor)
                .WithPageSize(request.PageSize)),
            cancellationToken).ToArrayAsync(cancellationToken);

        string keyCursor = operations[^1].Id.ToString();
        return new OperationHistory.Response.Success(
            new HistoryDto(
                operations.Select(record => record.MapToDto()).ToList()),
            keyCursor);
    }

    private async Task<Account?> FindAccountById(AccountId accountId, CancellationToken cancellationToken)
    {
        return await _accountRepository
            .QueryAsync(
                AccountQuery.Build(builder =>
                    builder.WithPageSize(1).WithAccountId(accountId)),
                cancellationToken)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<UserSession?> FindUserSessionById(SessionId sessionId, CancellationToken cancellationToken)
    {
        return await _userSessionRepository
            .QueryAsync(
                SessionQuery.Build(builder => builder.WithPageSize(1).WithSessionId(sessionId)),
                cancellationToken)
            .FirstOrDefaultAsync(cancellationToken);
    }
}