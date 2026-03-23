namespace BankApp.Cli.Infrastructure.BankApiService.Models;

public sealed class CreateAccount
{
    public sealed record Request(Guid SessionId, string PinCode);
}