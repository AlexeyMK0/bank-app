using Abstractions.Transactions;
using Lab1.Infrastructure.Persistence.Connections;
using Npgsql;
using System.Data;
using System.Data.Common;

namespace Lab1.Infrastructure.Persistence.PersistenceEntities;

public class PostgresDbSession : ITransactionProvider, IConnectionProvider
{
    private readonly NpgsqlDataSource _dataSource;

    private NpgsqlConnection? _connection;

    private NpgsqlTransaction? _currentTransaction;

    public PostgresDbSession(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken, IsolationLevel isolationLevel)
    {
        if (_currentTransaction is not null)
        {
            throw new InvalidOperationException("Transaction is already started");
        }

        _connection ??= await _dataSource.OpenConnectionAsync(cancellationToken);
        _currentTransaction = await _connection.BeginTransactionAsync(isolationLevel, cancellationToken);
        return new PostgresTransaction(_currentTransaction);
    }

    public async ValueTask<DbConnection> GetConnectionAsync(CancellationToken cancellationToken)
    {
        return _connection ??= await _dataSource.OpenConnectionAsync(cancellationToken);
    }

    public DbTransaction? CurrentTransaction => _currentTransaction;
}