namespace BankApp.Cli.Application.Abstractions.Operations;

public sealed class GetBalanceClient
{
    public readonly record struct Request(Guid SessionId);

    public abstract record Result
    {
        public sealed record Success(decimal Balance) : Result;

        public sealed record Failure(string Reason) : Result;
    }
}