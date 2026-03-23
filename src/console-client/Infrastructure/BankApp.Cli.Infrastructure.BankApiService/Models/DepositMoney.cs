namespace BankApp.Cli.Infrastructure.BankApiService.Models;

public sealed class DepositMoney
{
    public sealed record Request(decimal Amount, Guid SessionId);
}