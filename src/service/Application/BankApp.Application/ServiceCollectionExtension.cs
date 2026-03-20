using Abstractions.OperationHistory;
using Contracts.Accounts;
using Contracts.OperationHistory;
using Contracts.Sessions;
using Lab1.Application.Model;
using Lab1.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Lab1.Application;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IOperationHistoryService, OperationHistoryService>();
        services.AddScoped<IOperationHistoryWriter, OperationHistoryWriter>();

        services.AddOptions<DefaultIsolationLevel>()
            .BindConfiguration("Infrastructure:Persistence:DefaultOptions")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}