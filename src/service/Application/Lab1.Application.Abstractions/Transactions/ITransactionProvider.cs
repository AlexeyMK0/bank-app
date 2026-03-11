namespace Abstractions.Transactions;

public interface ITransactionProvider
{
    Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}