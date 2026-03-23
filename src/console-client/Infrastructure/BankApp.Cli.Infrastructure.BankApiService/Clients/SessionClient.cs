using BankApp.Cli.Application.Abstractions.Clients;
using BankApp.Cli.Application.Models;
using BankApp.Cli.Infrastructure.BankApiService.Models;
using BankApp.Cli.Infrastructure.BankApiService.RefitClients;
using Refit;
using CreateAdminSessionRequest = BankApp.Cli.Application.Abstractions.Operations.CreateAdminSessionRequest;
using CreateUserSessionRequest = BankApp.Cli.Application.Abstractions.Operations.CreateUserSessionRequest;

namespace BankApp.Cli.Infrastructure.BankApiService.Clients;

public class SessionClient : ISessionClient
{
    private readonly IRefitSessionClient _sessionClient;

    public SessionClient(IRefitSessionClient sessionClient)
    {
        _sessionClient = sessionClient;
    }

    public async Task<CreateUserSessionRequest.Result> CreateUserSessionAsync(CreateUserSessionRequest.Request request, CancellationToken cancellationToken)
    {
        var apiRequest = new CreateUserSession.Request(request.AccountId, request.PinCode);

        IApiResponse<CreateUserSession.Response> apiResponse =
            await _sessionClient.CreateUserSessionAsync(apiRequest, cancellationToken);
        if (!apiResponse.IsSuccessful)
        {
            return new CreateUserSessionRequest.Result.Failure(apiResponse.Error.Content ?? apiResponse.Error.Message);
        }

        CreateUserSession.Response content = apiResponse.Content;
        return new CreateUserSessionRequest.Result.Success(new UserSession(content.SessionId, content.AccountId));
    }

    public async Task<CreateAdminSessionRequest.Result> CreateAdminSessionAsync(CreateAdminSessionRequest.Request request, CancellationToken cancellationToken)
    {
        var apiRequest = new CreateAdminSession.Request(request.SystemPassword);

        IApiResponse<CreateAdminSession.Response> apiResponse =
            await _sessionClient.CreateAdminSessionAsync(apiRequest, cancellationToken);
        if (!apiResponse.IsSuccessful)
        {
            return new CreateAdminSessionRequest.Result.Failure(apiResponse.Error.Content ?? apiResponse.Error.Message);
        }

        CreateAdminSession.Response content = apiResponse.Content;
        return new CreateAdminSessionRequest.Result.Success(content.AdminSessionGuid);
    }
}