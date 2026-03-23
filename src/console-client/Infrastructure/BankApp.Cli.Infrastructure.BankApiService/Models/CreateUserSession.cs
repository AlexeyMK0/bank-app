namespace BankApp.Cli.Infrastructure.BankApiService.Models;

public record CreateUserSession
{
    public sealed record Request(long AccountId, string PinCode);

    public sealed record Response(Guid SessionId, long AccountId);
}