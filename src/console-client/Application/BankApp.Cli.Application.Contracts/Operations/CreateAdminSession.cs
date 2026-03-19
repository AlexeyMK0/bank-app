namespace BankApp.Cli.Application.Contracts.Operations;

public class CreateAdminSession
{
    public sealed record Request(string SystemPassword, bool StayLogged = false);

    public abstract record Result
    {
        public sealed record Success(Guid AdminSessionId) : Result;

        public sealed record Failure(string Reason) : Result;
    }
}