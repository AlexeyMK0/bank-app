using Lab1.Domain.Accounts;
using Lab1.Domain.Operations;
using Lab1.Domain.Sessions;
using Lab1.Domain.ValueObjects;

namespace Abstractions.OperationHistory;

public interface IOperationHistoryWriter
{
     Task<OperationRecord> AddOperationRecordAsync(
        OperationType type,
        AccountId accountId,
        SessionId sessionId,
        CancellationToken cancellationToken);
}