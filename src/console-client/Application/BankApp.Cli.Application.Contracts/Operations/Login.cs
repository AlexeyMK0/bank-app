namespace BankApp.Cli.Application.Contracts.Operations;

public class Login
{
    public sealed record Request(Guid SessionId);

    public abstract record Result
    {
        public sealed record Success : Result;

        public sealed record Failure(string Reason) : Result;
    }
}