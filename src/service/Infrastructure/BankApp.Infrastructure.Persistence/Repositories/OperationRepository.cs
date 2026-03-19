using Abstractions.Queries;
using Abstractions.Repositories;
using Lab1.Domain.Accounts;
using Lab1.Domain.Operations;
using Lab1.Domain.Sessions;
using Lab1.Domain.ValueObjects;
using Lab1.Infrastructure.Persistence.Connections;
using Lab1.Infrastructure.Persistence.PersistenceEntities;
using Npgsql;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace Lab1.Infrastructure.Persistence.Repositories;

public sealed class OperationRepository : IOperationRepository
{
    private readonly IConnectionProvider _dbSession;

    public OperationRepository(IConnectionProvider dbSession)
    {
        _dbSession = dbSession;
    }

    public async Task<OperationRecord> AddAsync(OperationRecord record, CancellationToken cancellationToken)
    {
        const string sql = """
                           INSERT INTO operations (operation_type, operation_time, account_id, session_guid)
                           VALUES (:operation_type, :operation_time, :account_id, :session_id)
                           RETURNING operation_id;
                           """;

        var connection = (NpgsqlConnection)await _dbSession.GetConnectionAsync(cancellationToken);
        var transaction = (NpgsqlTransaction?)_dbSession.CurrentTransaction;
        await using var command = new NpgsqlCommand(sql, connection, transaction);
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter("operation_type", record.Type.ToString()));
        command.Parameters.Add(new NpgsqlParameter("operation_time", record.Time.ToUniversalTime()));
        command.Parameters.Add(new NpgsqlParameter("account_id", record.AccountId.Value));
        command.Parameters.Add(new NpgsqlParameter("session_id", record.SessionId.Value));

        object? result = await command.ExecuteScalarAsync(cancellationToken);
        var newId = new OperationRecordId(Convert.ToInt64(result));
        return new OperationRecord(newId, record.Type, record.Time, record.AccountId, record.SessionId);
    }

    public async IAsyncEnumerable<OperationRecord> QueryAsync(
        OperationQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT operation_id, operation_type, operation_time, account_id, session_guid
                           FROM operations
                           WHERE (cardinality(:ids) = 0 or account_id = Any(:ids))
                           and (cardinality(:session_guids) = 0 or session_guid = Any(:session_guids))
                           and (:key_cursor::bigint IS NULL or operation_id > :key_cursor::bigint)
                           LIMIT :page_size
                           """;

        var connection = (NpgsqlConnection)await _dbSession.GetConnectionAsync(cancellationToken);
        var transaction = (NpgsqlTransaction?)_dbSession.CurrentTransaction;
        await using var command = new NpgsqlCommand(sql, connection, transaction);

        command.Parameters.Add(new NpgsqlParameter("page_size", query.PageSize));
        var keyCursorParam = new NpgsqlParameter("key_cursor", query.KeyCursor?.Value);
        keyCursorParam.Value ??= DBNull.Value;
        command.Parameters.Add(keyCursorParam);
        command.Parameters.Add(new NpgsqlParameter("ids", query.AccountIds.Select(entry => entry.Value).ToArray()));
        command.Parameters.Add(new NpgsqlParameter("session_guids", query.SessionIds.Select(entry => entry.Value).ToArray()));

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new OperationRecord(
                new OperationRecordId(reader.GetInt64("operation_id")),
                reader.MapEnum<OperationType>("operation_type"),
                await reader.GetFieldValueAsync<DateTimeOffset>("operation_time", cancellationToken),
                new AccountId(reader.GetInt64("account_id")),
                new SessionId(reader.GetGuid("session_guid")));
        }
    }
}