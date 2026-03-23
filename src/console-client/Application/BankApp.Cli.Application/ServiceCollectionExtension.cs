using BankApp.Cli.Application.Contracts.Services;
using BankApp.Cli.Application.Model;
using BankApp.Cli.Application.Model.Impl;
using BankApp.Cli.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BankApp.Cli.Application;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IUserContext>(new UserContext());
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ISessionService, SessionService>();

        return services;
    }
}