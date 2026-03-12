using Abstractions.Queries;
using Abstractions.Repositories;
using Lab1.Domain.Sessions;
using Lab1.Infrastructure.Persistence.Connections;
using Npgsql;

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
                           INSERT INTO user_sessions (session_guid)
                           VALUES (:session_guid)
                           """;
        var connection = (NpgsqlConnection)await _dbSession.GetConnectionAsync(cancellationToken);
        var transaction = (NpgsqlTransaction?)_dbSession.CurrentTransaction;
        await using var command = new NpgsqlCommand(sql, connection, transaction);
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter("session_guid", adminSession.Id.Value));

        return adminSession;
    }

    public IAsyncEnumerable<AdminSession> QueryAsync(SessionQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<AdminSession?> FindBySessionAsync(SessionId sessionId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}