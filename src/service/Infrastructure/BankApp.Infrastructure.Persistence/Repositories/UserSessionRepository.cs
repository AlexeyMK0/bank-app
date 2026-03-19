using Abstractions.Queries;
using Abstractions.Repositories;
using Lab1.Domain.Accounts;
using Lab1.Domain.Sessions;
using Lab1.Infrastructure.Persistence.Connections;
using Npgsql;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace Lab1.Infrastructure.Persistence.Repositories;

public sealed class UserSessionRepository : IUserSessionRepository
{
    private readonly IConnectionProvider _dbSession;

    public UserSessionRepository(IConnectionProvider dbSession)
    {
        _dbSession = dbSession;
    }

    public async Task<UserSession> AddAsync(UserSession userSession, CancellationToken cancellationToken)
    {
        const string sql = """
                           INSERT INTO user_sessions(session_guid, account_id)
                           VALUES (:session_guid, :account_id)
                           RETURNING session_id
                           """;

        var guid = Guid.NewGuid();
        var connection = (NpgsqlConnection)await _dbSession.GetConnectionAsync(cancellationToken);
        var transaction = (NpgsqlTransaction?)_dbSession.CurrentTransaction;
        await using var command = new NpgsqlCommand(sql, connection, transaction);
        command.CommandText = sql;

        command.Parameters.AddWithValue("session_guid", guid);
        command.Parameters.AddWithValue("account_id", userSession.AccountId.Value);

        long id = Convert.ToInt64(await command.ExecuteScalarAsync(cancellationToken));
        return new UserSession(id, new SessionId(guid), userSession.AccountId);
    }

    public async IAsyncEnumerable<UserSession> QueryAsync(
        SessionQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT session_id, session_guid, account_id
                           FROM user_sessions
                           WHERE (cardinality(:session_guids) = 0 or session_guid = ANY(:session_guids))
                           and (:key_cursor::BIGINT IS NULL or session_id > :key_cursor::BIGINT)
                           LIMIT :page_size
                           """;

        var connection = (NpgsqlConnection)await _dbSession.GetConnectionAsync(cancellationToken);
        var transaction = (NpgsqlTransaction?)_dbSession.CurrentTransaction;
        await using var command = new NpgsqlCommand(sql, connection, transaction);

        command.Parameters.Add(new NpgsqlParameter("page_size", query.PageSize));
        var keyCursorParam = new NpgsqlParameter("key_cursor", query.KeyCursor);
        keyCursorParam.Value ??= DBNull.Value;
        command.Parameters.Add(keyCursorParam);
        command.Parameters.Add(new NpgsqlParameter("session_guids", query.SessionIds.Select(entry => entry.Value).ToArray()));

        await using DbDataReader dataReader = await command.ExecuteReaderAsync(cancellationToken);
        while (await dataReader.ReadAsync(cancellationToken))
        {
            yield return new UserSession(
                dataReader.GetInt64("session_id"),
                new SessionId(dataReader.GetGuid("session_guid")),
                new AccountId(dataReader.GetInt64("account_id")));
        }
    }
}