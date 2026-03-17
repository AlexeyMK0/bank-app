using Abstractions.OperationHistory;
using Abstractions.Queries;
using Abstractions.Repositories;
using Abstractions.Transactions;
using Contracts.OperationHistory;
using Lab1.Application.Mappers;
using Lab1.Domain.Operations;
using Lab1.Domain.Sessions;
using Lab1.Domain.ValueObjects;
using System.Data;

namespace Lab1.Application.Services;

public class OperationHistoryService : IOperationHistoryService
{
    private const IsolationLevel IsolationLevel = System.Data.IsolationLevel.ReadCommitted;

    private readonly IUserSessionRepository _sessionRepository;
    private readonly IOperationRepository _operationRepository;
    private readonly ITransactionProvider _transactionProvider;

    private readonly IOperationHistoryWriter _operationWriter;

    public OperationHistoryService(
        IUserSessionRepository sessionRepository,
        IOperationRepository operationRepository,
        ITransactionProvider transactionProvider,
        IOperationHistoryWriter operationWriter)
    {
        _sessionRepository = sessionRepository;
        _operationRepository = operationRepository;
        _transactionProvider = transactionProvider;
        _operationWriter = operationWriter;
    }

    public async Task<GetAccountOperations.Response> GetAccountOperationsAsync(
        GetAccountOperations.Request request,
        CancellationToken cancellationToken)
    {
        UserSession? foundSession =
            await _sessionRepository.FindBySessionIdAsync(new SessionId(request.SenderSessionId), cancellationToken);
        if (foundSession is null)
        {
            return new GetAccountOperations.Response.Failure("Session not found");
        }

        OperationRecordId? inputKeyCursor =
            request.PageToken is null ? null : new OperationRecordId(request.PageToken.Token);

        OperationRecord[] operations = await _operationRepository.QueryAsync(
                OperationQuery.Build(builder => builder
                    .WithAccountIds([foundSession.AccountId])
                    .WithSessionIds([])
                    .WithKeyCursor(inputKeyCursor)
                    .WithPageSize(request.PageSize)),
                cancellationToken)
            .ToArrayAsync(cancellationToken);

        await using ITransaction transaction =
            await _transactionProvider.BeginTransactionAsync(cancellationToken, IsolationLevel);
        await _operationWriter.AddOperationRecordAsync(
            OperationType.GetHistory, foundSession.AccountId, foundSession.SessionGuid, cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        GetAccountOperations.PageToken? keyCursor = operations.Length > 0 ? new GetAccountOperations.PageToken(operations[^1].Id.Value) : null;
        return new GetAccountOperations.Response.Success(
            new HistoryDto(
                operations.Select(record => record.MapToDto()).ToList()),
            keyCursor);
    }
}