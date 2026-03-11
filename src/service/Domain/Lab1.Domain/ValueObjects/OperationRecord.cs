using Lab1.Domain.Accounts;
using Lab1.Domain.Sessions;

namespace Lab1.Domain.ValueObjects;

// TODO: create enum for operationType
public record OperationRecord(OperationRecordId Id, string OperationType, DateTimeOffset Time, AccountId AccountId, SessionId SessionId);