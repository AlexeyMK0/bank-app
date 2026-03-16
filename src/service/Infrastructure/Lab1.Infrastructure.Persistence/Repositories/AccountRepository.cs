using Abstractions.Queries;
using Abstractions.Repositories;
using Lab1.Domain.Accounts;
using Lab1.Domain.ValueObjects;
using Lab1.Infrastructure.Persistence.Connections;
using Npgsql;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace Lab1.Infrastructure.Persistence.Repositories;

public sealed class AccountRepository : IAccountRepository
{
    private readonly IConnectionProvider _dbSession;

    public AccountRepository(IConnectionProvider dbSession)
    {
        _dbSession = dbSession;
    }

    public async Task<Account> AddAsync(
        Account account,
        CancellationToken cancellationToken)
    {
        const string sql = """
           INSERT INTO "accounts" (balance, pincode)
           VALUES (:balance, :pincode)
           RETURNING id;
       """;

        var connection = (NpgsqlConnection)await _dbSession.GetConnectionAsync(cancellationToken);
        var transaction = (NpgsqlTransaction?)_dbSession.CurrentTransaction;
        await using var command = new NpgsqlCommand(sql, connection, transaction);
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter("balance", account.Balance.Value));
        command.Parameters.Add(new NpgsqlParameter("pincode", account.PinCode.Value));

        object? result = await command.ExecuteScalarAsync(cancellationToken);
        long newId = Convert.ToInt64(result);
        return new Account(
            new AccountId(newId), account.PinCode, account.Balance);
    }

    public async Task<Account> UpdateAsync(
        Account account,
        CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE accounts
            SET account_pincode = :pincode, account_balance = :balance
            WHERE account_id = :id";"
        """;

        var connection = (NpgsqlConnection)await _dbSession.GetConnectionAsync(cancellationToken);
        var transaction = (NpgsqlTransaction?)_dbSession.CurrentTransaction;
        await using var command = new NpgsqlCommand(sql, connection, transaction);
        command.Parameters.Add(new NpgsqlParameter("balance", account.Balance.Value));
        command.Parameters.Add(new NpgsqlParameter("pincode", account.PinCode.Value));

        await command.ExecuteNonQueryAsync(cancellationToken);
        return account;
    }

    public async IAsyncEnumerable<Account> QueryAsync(
        AccountQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
           SELECT id, pincode, balance
           FROM accounts
           WHERE (
               (cardinality(:ids) = 0 or account_id = ANY(:ids))
                and (:key_cursor IS NULL or account_id > :key_cursor))
           LIMIT :page_size;
        """;

        long[] ids = query.AccountIds.Select(id => id.Value).ToArray();
        var connection = (NpgsqlConnection)await _dbSession.GetConnectionAsync(cancellationToken);
        var transaction = (NpgsqlTransaction?)_dbSession.CurrentTransaction;
        await using var command = new NpgsqlCommand(sql, connection, transaction);
        command.Parameters.Add(new NpgsqlParameter("ids", ids));
        command.Parameters.Add(new NpgsqlParameter("key_cursor", long.Parse(query.KeyCursor ?? "-1")));
        command.Parameters.Add(new NpgsqlParameter("page_size", query.PageSize));

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new Account(
                new AccountId(reader.GetInt64("id")),
                new PinCode(reader.GetString("pincode")),
                new Money(reader.GetDecimal("balance")));
        }
    }
}