using BankApp.Cli.Application.Abstractions.Clients;
using BankApp.Cli.Application.Abstractions.Operations;
using BankApp.Cli.Application.Models;
using BankApp.Cli.Infrastructure.BankApiService.Models;
using BankApp.Cli.Infrastructure.BankApiService.RefitClients;
using Refit;

namespace BankApp.Cli.Infrastructure.BankApiService.Clients;

public class SessionClient : ISessionClient
{
    private readonly IRefitSessionClient _sessionClient;

    public SessionClient(IRefitSessionClient sessionClient)
    {
        _sessionClient = sessionClient;
    }

    public async Task<CreateUserSessionClient.Result> CreateUserSessionAsync(CreateUserSessionClient.Request request, CancellationToken cancellationToken)
    {
        var apiRequest = new CreateUserSessionRequest(request.AccountId, request.PinCode);

        IApiResponse<CreateUserSessionResponse> apiResponse =
            await _sessionClient.CreateUserSessionAsync(apiRequest, cancellationToken);
        if (!apiResponse.IsSuccessful)
        {
            return new CreateUserSessionClient.Result.Failure(apiResponse.ReasonPhrase ?? apiResponse.Error.Message);
        }

        CreateUserSessionResponse content = apiResponse.Content;
        return new CreateUserSessionClient.Result.Success(new UserSession(content.SessionId, content.AccountId));
    }

    public async Task<CreateAdminSessionClient.Result> CreateAdminSessionAsync(CreateAdminSessionClient.Request request, CancellationToken cancellationToken)
    {
        var apiRequest = new CreateAdminSessionRequest(request.SystemPassword);

        IApiResponse<CreateAdminSessionResponse> apiResponse =
            await _sessionClient.CreateAdminSessionAsync(apiRequest, cancellationToken);
        if (!apiResponse.IsSuccessful)
        {
            return new CreateAdminSessionClient.Result.Failure(apiResponse.ReasonPhrase ?? apiResponse.Error.Message);
        }

        CreateAdminSessionResponse content = apiResponse.Content;
        return new CreateAdminSessionClient.Result.Success(content.AdminSessionGuid);
    }
}