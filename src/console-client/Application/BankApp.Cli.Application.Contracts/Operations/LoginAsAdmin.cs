namespace BankApp.Cli.Application.Contracts.Operations;

public class LoginAsAdmin
{
    public sealed record Request(string SystemPassword);

    public abstract record Result
    {
        public sealed record Success : Result;

        public sealed record Failure(string Reason) : Result;
    }
}