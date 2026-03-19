namespace BankApp.Cli.Application.Contracts.Operations;

public class Logout
{
    public abstract record Result
    {
        public sealed record Success : Result;

        public sealed record Failure(string Reason) : Result;
    }
}