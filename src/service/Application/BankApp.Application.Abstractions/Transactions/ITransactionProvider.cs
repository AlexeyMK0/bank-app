using System.Data;

namespace Abstractions.Transactions;

public interface ITransactionProvider
{
    Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken, IsolationLevel isolationLevel);
}