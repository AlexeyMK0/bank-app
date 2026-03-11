using Abstractions.Queries;
using Abstractions.Repositories;
using Lab1.Domain.Accounts;

namespace Lab1.Infrastructure.Persistence.Repositories;

public sealed class AccountRepository : IAccountRepository
{
    public Task<Account> AddAsync(Account account, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Account> UpdateAsync(Account account, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<Account> QueryAsync(AccountQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}