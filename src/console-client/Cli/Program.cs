using BankApp.Cli.Application;
using BankApp.Cli.Application.Contracts.Services;
using BankApp.Cli.Infrastructure.BankApiService;
using BankApp.Cli.Presentation.Cli;
using BankApp.Cli.Presentation.Cli.CommandHandlers;
using BankApp.Cli.Presentation.Cli.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;

IConfigurationRoot configuration = new ConfigurationManager()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build();

IServiceCollection services = new ServiceCollection()
    .AddApplication()
    .AddBankApiClients()
    .AddPresentation()
    .AddSingleton<IConfiguration>(configuration);

var registrar = new ServiceCollectionRegistrar(services);

var app = new CommandApp(registrar);

app.Configure(config =>
{
    config
        .AddCommand<CreateAccountCommandHandler>("create-account")
        .WithDescription("Creates a new account");

    config
        .AddCommand<DepositMoneyCommandHandler>("deposit")
        .WithDescription("Deposits money on selected account");

    config
        .AddCommand<GetBalanceCommandHandler>("balance")
        .WithDescription("Gets balance of selected account");

    config
        .AddCommand<GetHistoryCommandHandler>("get-history")
        .WithDescription("Gets operation history of selected account");

    config
        .AddCommand<LoginAsAdminCommandHandler>("login-admin")
        .WithDescription("Logins as admin account");

    config
        .AddCommand<LoginAsUserCommandHandler>("login-user")
        .WithDescription("Logins as user account");

    config
        .AddCommand<LogoutCommandHandler>("logout")
        .WithDescription("Logouts of account");

    config
        .AddCommand<WithdrawMoneyCommandHandler>("withdraw")
        .WithDescription("Withdraws money from selected account");

    config
        .AddCommand<ExitCommandHandler>("exit");
});

ServiceProvider serviceProvider = services.BuildServiceProvider();
serviceProvider.GetRequiredService<ConsolePrinter>();
serviceProvider.GetRequiredService<ISessionService>();

AppLifetimeManager lifetimeManager = serviceProvider.GetRequiredService<AppLifetimeManager>();

AnsiConsole.Write(new FigletText("Bank App"));
AnsiConsole.WriteLine();
AnsiConsole.WriteLine("Enter --help to see available commands");
AnsiConsole.WriteLine();

while (lifetimeManager.IsExisting)
{
    string line = AnsiConsole.Ask<string>(">");

    await app.RunAsync(line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
    AnsiConsole.WriteLine();
}
