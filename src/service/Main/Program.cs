// See https://aka.ms/new-console-template for more information

using FluentMigrator.Runner;
using Lab1.Application;
using Lab1.Infrastructure.Persistence;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPersistence(builder.Configuration)
    .AddApplication();

WebApplication app = builder.Build();

await using (AsyncServiceScope scope = app.Services.CreateAsyncScope())
{
    IMigrationRunner migrationRunner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    migrationRunner.MigrateUp();
}

await app.RunAsync();