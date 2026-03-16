using Contracts.Accounts;
using Contracts.Sessions;
using Lab1.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Lab1.Application;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ISessionService, SessionService>();

        return services;
    }
}