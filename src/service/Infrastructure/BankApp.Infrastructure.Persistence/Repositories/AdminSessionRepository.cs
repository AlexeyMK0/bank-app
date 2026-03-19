using Abstractions.Queries;
using Abstractions.Repositories;
using Lab1.Domain.Sessions;
using Lab1.Infrastructure.Persistence.Connections;
using Npgsql;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace Lab1.Infrastructure.Persistence.Repositories;

public sealed class AdminSessionRepository : IAdminSessionRepository
{
    private readonly IConnectionProvider _dbSession;

    public AdminSessionRepository(IConnectionProvider dbSession)
    {
        _dbSession = dbSession;
    }

    public async Task<AdminSession> AddAsync(AdminSession adminSession, CancellationToken cancellationToken)
    {
        const string sql = """
                           INSERT INTO admin_sessions (session_guid)
                           VALUES (:session_guid)
                           """;

        var guid = new SessionId(Guid.NewGuid());

        var connection = (NpgsqlConnection)await _dbSession.GetConnectionAsync(cancellationToken);
        var transaction = (NpgsqlTransaction?)_dbSession.CurrentTransaction;
        await using var command = new NpgsqlCommand(sql, connection, transaction);
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter("session_guid", guid.Value));

        object? result = await command.ExecuteScalarAsync(cancellationToken);
        return new AdminSession(Convert.ToInt64(result), guid);
    }

    public async IAsyncEnumerable<AdminSession> QueryAsync(
        SessionQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT session_id, session_guid
                           FROM admin_sessions
                           WHERE (cardinality(:ids) = 0 or session_guid = ANY(:ids))
                           and (:key_cursor::bigint is NULL or session_id > :key_cursor::bigint)
                           LIMIT :page_size
                           """;

        Guid[] ids = query.SessionIds.Select(sessionId => sessionId.Value).ToArray();
        int? keyCursor = query.KeyCursor is null ? null : Convert.ToInt32(query.KeyCursor);

        var connection = (NpgsqlConnection)await _dbSession.GetConnectionAsync(cancellationToken);
        var transaction = (NpgsqlTransaction?)_dbSession.CurrentTransaction;
        await using var command = new NpgsqlCommand(sql, connection, transaction);
        var keyCursorParam = new NpgsqlParameter("key_cursor", keyCursor);
        keyCursorParam.Value ??= DBNull.Value;

        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter("ids", ids));
        command.Parameters.Add(keyCursorParam);
        command.Parameters.Add(new NpgsqlParameter("page_size", Convert.ToInt32(query.PageSize)));
        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new AdminSession(
                reader.GetInt64("session_id"),
                new SessionId(reader.GetGuid("session_guid")));
        }
    }
}