namespace BankApp.Cli.Application.Abstractions.Operations;

public sealed class WithdrawMoneyRequest
{
    public readonly record struct Request(decimal Amount, Guid SessionId);

    public abstract record Result
    {
        public sealed record Success(decimal UpdatedBalance) : Result;

        public sealed record Failure(string Reason) : Result;
    }
}