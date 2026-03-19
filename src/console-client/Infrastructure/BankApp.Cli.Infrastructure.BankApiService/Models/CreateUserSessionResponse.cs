namespace BankApp.Cli.Infrastructure.BankApiService.Models;

public record CreateUserSessionResponse(Guid SessionId, long AccountId);