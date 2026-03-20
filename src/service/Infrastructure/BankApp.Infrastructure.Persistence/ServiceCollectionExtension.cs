using Abstractions.Password;
using Abstractions.Repositories;
using Abstractions.Transactions;
using FluentMigrator.Runner;
using Lab1.Infrastructure.Persistence.Connections;
using Lab1.Infrastructure.Persistence.Password;
using Lab1.Infrastructure.Persistence.PersistenceEntities;
using Lab1.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Lab1.Infrastructure.Persistence;

public static class ServiceCollectionExtension
{
    private static readonly ILogger Logger = LoggerFactory
        .Create(builder => builder.AddConsole())
        .CreateLogger("ServiceCollectionExtension console");

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPasswordProvider, PasswordProvider>();

        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IAdminSessionRepository, AdminSessionRepository>();
        services.AddScoped<IOperationRepository, OperationRepository>();
        services.AddScoped<IUserSessionRepository, UserSessionRepository>();

        services.AddScoped<PostgresDbSession>();
        services.AddScoped<IConnectionProvider>(serviceProvider =>
            serviceProvider.GetRequiredService<PostgresDbSession>());
        services.AddScoped<ITransactionProvider>(serviceProvider =>
            serviceProvider.GetRequiredService<PostgresDbSession>());

        var builder = new NpgsqlConnectionStringBuilder();
        configuration.GetRequiredSection("Infrastructure:Persistence:Postgres").Bind(builder);
        string connectionString = builder.ConnectionString;

        Logger.LogInformation("Postgres connection string: " + connectionString);

        services.AddSingleton(NpgsqlDataSource.Create(connectionString));

        services.AddFluentMigratorCore()
            .ConfigureRunner(runner => runner
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .WithMigrationsIn(typeof(IMigrationsAssemblyMarker).Assembly));

        services.AddOptions<PasswordOptions>()
            .BindConfiguration("SystemPasswordSettings")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}