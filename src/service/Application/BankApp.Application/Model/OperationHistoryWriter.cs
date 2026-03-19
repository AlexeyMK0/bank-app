using Abstractions.OperationHistory;
using Abstractions.Repositories;
using Lab1.Domain.Accounts;
using Lab1.Domain.Operations;
using Lab1.Domain.Sessions;
using Lab1.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Lab1.Application.Model;

public sealed class OperationHistoryWriter : IOperationHistoryWriter
{
    private static readonly ILogger Logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<OperationHistoryWriter>();

    private readonly IOperationRepository _operationRepository;

    public OperationHistoryWriter(IOperationRepository operationRepository)
    {
        _operationRepository = operationRepository;
    }

    public async Task<OperationRecord> AddOperationRecordAsync(
        OperationType type,
        AccountId accountId,
        SessionId sessionId,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Adding operation record");

        var operationHistoryRecord = new OperationRecord(
            OperationRecordId.Default,
            type,
            DateTimeOffset.Now,
            accountId,
            sessionId);

        Logger.LogDebug("With parameters: " + operationHistoryRecord);

        OperationRecord operationRecord = await _operationRepository.AddAsync(operationHistoryRecord, cancellationToken);

        return operationRecord;
    }
}