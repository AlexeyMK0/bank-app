using BankApp.Cli.Application.Abstractions.Clients;
using BankApp.Cli.Infrastructure.BankApiService.Clients;
using BankApp.Cli.Infrastructure.BankApiService.Options;
using BankApp.Cli.Infrastructure.BankApiService.RefitClients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;

namespace BankApp.Cli.Infrastructure.BankApiService;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddBankApiClients(this IServiceCollection services)
    {
        services
            .AddOptions<BankApiClientsOptions>()
            .BindConfiguration("Infrastructure:Configuration")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddRefitClient<IRefitAccountClient>()
            .ConfigureHttpClient((serviceProvider, httpClient) =>
            {
                BankApiClientsOptions options
                    = serviceProvider.GetRequiredService<IOptions<BankApiClientsOptions>>().Value;
                httpClient.BaseAddress = options.ConnectionUri;
            });

        services
            .AddRefitClient<IRefitSessionClient>()
            .ConfigureHttpClient((serviceProvider, httpClient) =>
            {
                BankApiClientsOptions options
                    = serviceProvider.GetRequiredService<IOptions<BankApiClientsOptions>>().Value;
                httpClient.BaseAddress = options.ConnectionUri;
            });

        services.AddScoped<ISessionClient, SessionClient>();
        services.AddScoped<IAccountClient, AccountClient>();
        return services;
    }
}