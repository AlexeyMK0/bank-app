using BankApp.Cli.Presentation.Cli.Model;
using BankApp.Cli.Presentation.Cli.Options;
using Microsoft.Extensions.DependencyInjection;

namespace BankApp.Cli.Presentation.Cli;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddOptions<CliOptions>()
            .BindConfiguration("CliOptions")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<ConsolePrinter>();
        services.AddSingleton(new AppLifetimeManager());
        return services;
    }
}