using BankApp.Cli.Application.Abstractions.Operations;

namespace BankApp.Cli.Application.Abstractions.Clients;

public interface ISessionClient
{
    Task<CreateUserSessionClient.Result> CreateUserSessionAsync(
        CreateUserSessionClient.Request request,
        CancellationToken cancellationToken);

    Task<CreateAdminSessionClient.Result> CreateAdminSessionAsync(
        CreateAdminSessionClient.Request request,
        CancellationToken cancellationToken);
}