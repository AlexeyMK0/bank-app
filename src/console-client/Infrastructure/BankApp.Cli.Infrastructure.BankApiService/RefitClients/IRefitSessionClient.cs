using BankApp.Cli.Infrastructure.BankApiService.Models;
using Refit;

namespace BankApp.Cli.Infrastructure.BankApiService.RefitClients;

public interface IRefitSessionClient
{
    [Post("/api/session/user")]
    Task<IApiResponse<CreateUserSession.Response>> CreateUserSessionAsync(
        [Body] CreateUserSession.Request request,
        CancellationToken cancellationToken);

    [Post("/api/session/admin")]
    Task<IApiResponse<CreateAdminSession.Response>> CreateAdminSessionAsync(
        [Body] CreateAdminSession.Request request,
        CancellationToken cancellationToken);
}