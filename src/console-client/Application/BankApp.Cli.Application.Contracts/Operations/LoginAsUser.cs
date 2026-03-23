namespace BankApp.Cli.Application.Contracts.Operations;

public class LoginAsUser
{
    public sealed record Request();

    public abstract record Result
    {
        public sealed record Success : Result;

        public sealed record Failure(string Reason) : Result;
    }
}