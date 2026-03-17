using Abstractions.OperationHistory;
using Abstractions.Password;
using Abstractions.Queries;
using Abstractions.Repositories;
using Abstractions.Transactions;
using Contracts.Sessions;
using Contracts.Sessions.Operations;
using Lab1.Application.Mappers;
using Lab1.Application.Model;
using Lab1.Domain.Accounts;
using Lab1.Domain.Operations;
using Lab1.Domain.Sessions;
using Lab1.Domain.ValueObjects;
using Microsoft.Extensions.Options;
using System.Data;

namespace Lab1.Application.Services;

public sealed class SessionService : ISessionService
{
    private readonly IsolationLevel _isolationLevel;

    private readonly IAccountRepository _accounts;
    private readonly IUserSessionRepository _users;
    private readonly IAdminSessionRepository _adminSessions;
    private readonly IPasswordProvider _passwordProvider;
    private readonly IOperationHistoryWriter _operationWriter;
    private readonly ITransactionProvider _transactionProvider;

    public SessionService(
        IAccountRepository repository,
        IUserSessionRepository users,
        IPasswordProvider passwordProvider,
        IAdminSessionRepository adminSessions,
        ITransactionProvider transactionProvider,
        IOptions<DefaultIsolationLevel> isolationLevelOptions,
        IOperationHistoryWriter operationWriter)
    {
        _accounts = repository;
        _users = users;
        _passwordProvider = passwordProvider;
        _adminSessions = adminSessions;
        _transactionProvider = transactionProvider;
        _operationWriter = operationWriter;
        _isolationLevel = isolationLevelOptions.Value.IsolationLevel;
    }

    public async Task<CreateUserSession.Response> CreateUserSessionAsync(CreateUserSession.Request request, CancellationToken cancellationToken)
    {
        var pinCode = new PinCode(request.PinCode);
        var accountId = new AccountId(request.AccountId);

        Account? account = await
            _accounts.QueryAsync(
                    AccountQuery.Build(builder => builder.WithAccountId(accountId).WithPageSize(1)),
                    cancellationToken)
            .FirstOrDefaultAsync(cancellationToken);

        if (account is null)
            return new CreateUserSession.Response.Failure($"Account {accountId.Value} not found");

        if (account.PinCode != pinCode)
            return new CreateUserSession.Response.Failure("Wrong pin code");

        await using ITransaction transaction =
            await _transactionProvider.BeginTransactionAsync(cancellationToken, _isolationLevel);

        var session = new UserSession(null, SessionId.Default, accountId);
        session = await _users.AddAsync(session, cancellationToken);

        await _operationWriter.AddOperationRecordAsync(
            OperationType.CreateUserSession, accountId, session.SessionGuid, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return new CreateUserSession.Response.Success(session.MapToDto());
    }

    public async Task<CreateAdminSession.Response> CreateAdminSessionAsync(CreateAdminSession.Request request, CancellationToken cancellationToken)
    {
        if (_passwordProvider.Password != request.SystemPassword)
        {
            return new CreateAdminSession.Response.Failure("Wrong password");
        }

        await using ITransaction transaction =
            await _transactionProvider.BeginTransactionAsync(cancellationToken, _isolationLevel);

        var adminSession = new AdminSession(null, SessionId.Default);
        adminSession = await _adminSessions.AddAsync(adminSession, cancellationToken);

        await _operationWriter.AddOperationRecordAsync(
            OperationType.CreateAdminSession, AccountId.Default, adminSession.SessionGuid, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return new CreateAdminSession.Response.Success(adminSession.SessionGuid.Value);
    }
}