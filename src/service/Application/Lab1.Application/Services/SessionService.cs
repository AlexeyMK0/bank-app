using Abstractions.Password;
using Abstractions.Queries;
using Abstractions.Repositories;
using Contracts.Sessions;
using Contracts.Sessions.Operations;
using Lab1.Application.Mappers;
using Lab1.Domain.Accounts;
using Lab1.Domain.Sessions;
using Lab1.Domain.ValueObjects;

namespace Lab1.Application.Services;

public sealed class SessionService : ISessionService
{
    private readonly IAccountRepository _accounts;
    private readonly IUserSessionRepository _users;
    private readonly IAdminSessionRepository _adminSessions;
    private readonly IPasswordProvider _passwordProvider;

    public SessionService(IAccountRepository repository, IUserSessionRepository users, IPasswordProvider passwordProvider, IAdminSessionRepository adminSessions)
    {
        _accounts = repository;
        _users = users;
        _passwordProvider = passwordProvider;
        _adminSessions = adminSessions;
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

        var session = new UserSession(SessionId.Default, accountId);
        session = await _users.AddAsync(session, cancellationToken);
        return new CreateUserSession.Response.Success(session.MapToDto());
    }

    public async Task<CreateAdminSession.Response> CreateAdminSessionAsync(CreateAdminSession.Request request, CancellationToken cancellationToken)
    {
        if (_passwordProvider.Password != request.SystemPassword)
        {
            return new CreateAdminSession.Response.Failure("Wrong password");
        }

        var adminSession = new AdminSession(SessionId.Default);
        adminSession = await _adminSessions.AddAsync(adminSession, cancellationToken);

        return new CreateAdminSession.Response.Success(adminSession.Id.Value);
    }
}