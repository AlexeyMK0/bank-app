namespace BankApp.Cli.Application.Contracts.Operations;

public class WithdrawMoney
{
    public sealed record Request(decimal Amount);

    public abstract record Result
    {
        public sealed record Success(decimal UpdatedBalance) : Result;

        public sealed record Failure(string Reason) : Result;
    }
}