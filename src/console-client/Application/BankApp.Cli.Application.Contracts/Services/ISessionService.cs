using BankApp.Cli.Application.Contracts.Operations;

namespace BankApp.Cli.Application.Contracts.Services;

public interface ISessionService
{
    Task<CreateUserSession.Result> CreateUserSessionAsync(
        CreateUserSession.Request request,
        CancellationToken cancellationToken);

    Task<CreateAdminSession.Result> CreateAdminSessionAsync(
        CreateAdminSession.Request request,
        CancellationToken cancellationToken);

    Logout.Result Logout();
}