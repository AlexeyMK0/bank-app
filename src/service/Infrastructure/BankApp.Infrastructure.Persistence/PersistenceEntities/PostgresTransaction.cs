using Abstractions.Transactions;
using Npgsql;

namespace Lab1.Infrastructure.Persistence.PersistenceEntities;

public class PostgresTransaction : ITransaction
{
    private readonly NpgsqlTransaction _transaction;

    public PostgresTransaction(NpgsqlTransaction transaction)
    {
        _transaction = transaction;
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        await _transaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken)
    {
        await _transaction.RollbackAsync(cancellationToken);
    }

    public void Dispose()
    {
        _transaction.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _transaction.DisposeAsync();
    }
}