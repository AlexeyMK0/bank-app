namespace BankApp.Cli.Infrastructure.BankApiService.Models;

public record CreateUserSessionRequest(long AccountId, string PinCode);