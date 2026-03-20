namespace BankApp.Cli.Infrastructure.BankApiService.Models;

public record GetHistoryReponse(
    IReadOnlyCollection<GetHistoryReponse.OperationRecord> Operations,
    string? PageToken)
{
    public sealed record OperationRecord(
        string OperationType,
        DateTimeOffset Time,
        long AccountId,
        Guid SessionId);
}