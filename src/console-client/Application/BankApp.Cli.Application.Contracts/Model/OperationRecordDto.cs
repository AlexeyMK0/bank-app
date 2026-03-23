namespace BankApp.Cli.Application.Contracts.Model;

public record OperationRecordDto(string OperationType, DateTimeOffset Time, long AccountId, Guid SessionId);