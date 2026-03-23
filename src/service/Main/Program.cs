using Lab1.Application;
using Lab1.Infrastructure.Persistence;
using Lab1.Presentation.Http;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPersistence(builder.Configuration)
    .AddApplication()
    .AddPresentationHttp();

builder.Services.AddLogging(loggerBuilder => loggerBuilder.AddConsole());

builder.Services.AddSwaggerGen().AddEndpointsApiExplorer();

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.MapControllers();

await app.RunAsync();