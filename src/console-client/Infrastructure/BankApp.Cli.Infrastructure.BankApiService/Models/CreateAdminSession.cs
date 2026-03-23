namespace BankApp.Cli.Infrastructure.BankApiService.Models;

public sealed class CreateAdminSession
{
    public sealed record Request(string SystemPassword);

    public sealed record Response(Guid AdminSessionGuid);
}