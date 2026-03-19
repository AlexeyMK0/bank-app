namespace BankApp.Cli.Infrastructure.BankApiService.Models;

public record CreateAccountRequest(Guid SessionId, string PinCode);