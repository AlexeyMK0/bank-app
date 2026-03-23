namespace BankApp.Cli.Infrastructure.BankApiService.Models;

public sealed class GetHistory
{
    public sealed record Response(
        IReadOnlyCollection<OperationRecord> Operations,
        string? PageToken);

    public sealed record OperationRecord(
        string OperationType,
        DateTimeOffset Time,
        long AccountId,
        Guid SessionId);
}