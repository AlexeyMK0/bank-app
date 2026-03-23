namespace BankApp.Cli.Application.Contracts.Operations;

public class GetBalance
{
    public sealed record Request;

    public abstract record Result
    {
        public sealed record Success(decimal Balance) : Result;

        public sealed record Failure(string Reason) : Result;
    }
}