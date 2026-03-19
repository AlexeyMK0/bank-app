namespace BankApp.Cli.Application.Contracts.Operations;

public class DepositMoney
{
    public sealed record Request(Guid SessionId, decimal Amount);

    public abstract record Result
    {
        public sealed record Success(decimal UpdatedBalance) : Result;

        public sealed record Failure(string Reason) : Result;
    }
}