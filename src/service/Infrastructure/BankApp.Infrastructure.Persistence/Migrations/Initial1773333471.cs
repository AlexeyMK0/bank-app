using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;

namespace Lab1.Infrastructure.Persistence.Migrations;

// TODO: create different migrations
[Migration(1773333471, "Initial")]
public class Initial1773333471 : IMigration
{
    public void GetUpExpressions(IMigrationContext context)
    {
        context.Expressions.Add(new ExecuteSqlStatementExpression
        {
            SqlStatement = """
               CREATE TABLE accounts
               (
                    account_id BIGSERIAL NOT NULL PRIMARY KEY,
                    account_pincode TEXT NOT NULL,
                    account_balance decimal NOT NULL
               );

                CREATE TABLE operations
                (
                    operation_id BIGSERIAL NOT NULL PRIMARY KEY,
                    operation_type TEXT NOT NULL,
                    operation_time timestamp with time zone NOT NULL,
                    account_id BIGSERIAL NOT NULL,
                    session_guid UUID NOT NULL,
                    
                    FOREIGN KEY (account_id) REFERENCES accounts(account_id)
                );

                CREATE TABLE admin_sessions
                (
                    session_id BIGSERIAL NOT NULL PRIMARY KEY,
                    session_guid UUID NOT NULL UNIQUE
                );

                CREATE TABLE user_sessions
                (
                    session_id BIGSERIAL NOT NULL PRIMARY KEY,
                    session_guid UUID NOT NULL,
                    account_id BIGSERIAL NOT NULL,
                    
                    FOREIGN KEY (account_id) REFERENCES accounts(account_id)
                );
               """,
        });
    }

    public void GetDownExpressions(IMigrationContext context)
    {
        context.Expressions.Add(new ExecuteSqlStatementExpression
        {
            SqlStatement = """
               DROP TABLE user_sessions;
               DROP TABLE admin_sessions;
               DROP TABLE operations;
               DROP TABLE accounts;
               """,
        });
    }

    public string ConnectionString => throw new NotSupportedException();
}