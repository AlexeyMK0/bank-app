namespace BankApp.Cli.Application.Models;

public record OperationRecord(
    string Type,
    DateTimeOffset Time,
    long AccountId,
    Guid SessionId);