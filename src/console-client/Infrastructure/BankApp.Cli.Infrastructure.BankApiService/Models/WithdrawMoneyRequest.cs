namespace BankApp.Cli.Infrastructure.BankApiService.Models;

public record WithdrawMoneyRequest(decimal Amount, Guid SessionId);