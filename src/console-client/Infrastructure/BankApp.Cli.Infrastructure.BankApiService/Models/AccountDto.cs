namespace BankApp.Cli.Infrastructure.BankApiService.Models;

public record AccountDto(long AccountId, decimal Balance, string PinCode);