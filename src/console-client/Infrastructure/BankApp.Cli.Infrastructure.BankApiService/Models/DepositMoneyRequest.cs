namespace BankApp.Cli.Infrastructure.BankApiService.Models;

public record DepositMoneyRequest(decimal Amount, Guid SessionId);