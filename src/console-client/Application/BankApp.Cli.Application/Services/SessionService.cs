using BankApp.Cli.Application.Abstractions.Clients;
using BankApp.Cli.Application.Abstractions.Operations;
using BankApp.Cli.Application.Contracts.Operations;
using BankApp.Cli.Application.Contracts.Services;
using BankApp.Cli.Application.Model;
using System.Diagnostics;

namespace BankApp.Cli.Application.Services;

public class SessionService : ISessionService
{
    private readonly IUserContext _userContext;
    private readonly ISessionClient _sessionClient;

    public SessionService(IUserContext userContext, ISessionClient sessionClient)
    {
        _userContext = userContext;
        _sessionClient = sessionClient;
    }

    public async Task<CreateUserSession.Result> CreateUserSessionAsync(
        CreateUserSession.Request request,
        CancellationToken cancellationToken)
    {
        if (request.StayLogged is true && _userContext.CurrentSession is not null)
        {
            return new CreateUserSession.Result.Failure("You are already logged in");
        }

        CreateUserSessionClient.Result result = await _sessionClient.CreateUserSessionAsync(
            new CreateUserSessionClient.Request(request.AccountId, request.PinCode),
            cancellationToken);

        switch (result)
        {
            case CreateUserSessionClient.Result.Success success:
                if (request.StayLogged is true)
                {
                    _userContext.CurrentSession = success.CreatedSession.SessionId;
                }

                return new CreateUserSession.Result.Success();
            case CreateUserSessionClient.Result.Failure failure:
                return new CreateUserSession.Result.Failure(failure.Reason);
            default:
                throw new UnreachableException();
        }
    }

    public async Task<CreateAdminSession.Result> CreateAdminSessionAsync(
        CreateAdminSession.Request request,
        CancellationToken cancellationToken)
    {
        if (request.StayLogged is true && _userContext.CurrentSession is not null)
        {
            return new CreateAdminSession.Result.Failure("You are already logged in");
        }

        CreateAdminSessionClient.Result result = await _sessionClient.CreateAdminSessionAsync(
            new CreateAdminSessionClient.Request(request.SystemPassword),
            cancellationToken);

        switch (result)
        {
            case CreateAdminSessionClient.Result.Success success:
                if (request.StayLogged is true)
                    _userContext.CurrentSession = success.CreatedSessionId;
                return new CreateAdminSession.Result.Success();
            case CreateAdminSessionClient.Result.Failure failure:
                return new CreateAdminSession.Result.Failure(failure.Reason);
            default:
                throw new UnreachableException();
        }
    }

    public Logout.Result Logout()
    {
        if (_userContext.CurrentSession is null)
        {
            return new Logout.Result.Failure("You are not logged in");
        }

        _userContext.CurrentSession = null;
        return new Logout.Result.Success();
    }
}