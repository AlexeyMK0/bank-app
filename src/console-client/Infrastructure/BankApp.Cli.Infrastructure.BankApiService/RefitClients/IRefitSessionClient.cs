using BankApp.Cli.Infrastructure.BankApiService.Models;
using Refit;

namespace BankApp.Cli.Infrastructure.BankApiService.RefitClients;

public interface IRefitSessionClient
{
    [Post("/api/session/user")]
    Task<IApiResponse<CreateUserSessionResponse>> CreateUserSessionAsync(
        [Body] CreateUserSessionRequest request,
        CancellationToken cancellationToken);

    [Post("/api/session/admin")]
    Task<IApiResponse<CreateAdminSessionResponse>> CreateAdminSessionAsync(
        [Body] CreateAdminSessionRequest request,
        CancellationToken cancellationToken);
}