using BankApp.Cli.Application.Abstractions.Operations;

namespace BankApp.Cli.Application.Abstractions.Clients;

public interface ISessionClient
{
    Task<CreateUserSessionRequest.Result> CreateUserSessionAsync(
        CreateUserSessionRequest.Request request,
        CancellationToken cancellationToken);

    Task<CreateAdminSessionRequest.Result> CreateAdminSessionAsync(
        CreateAdminSessionRequest.Request request,
        CancellationToken cancellationToken);
}