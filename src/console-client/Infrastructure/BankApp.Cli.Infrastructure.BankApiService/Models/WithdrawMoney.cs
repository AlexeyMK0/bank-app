namespace BankApp.Cli.Infrastructure.BankApiService.Models;

public sealed class WithdrawMoney
{
    public sealed record Request(decimal Amount, Guid SessionId);
}