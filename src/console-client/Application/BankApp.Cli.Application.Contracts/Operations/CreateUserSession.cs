namespace BankApp.Cli.Application.Contracts.Operations;

public class CreateUserSession
{
    public sealed record Request(long AccountId, string PinCode, bool StayLogged = true);

    public abstract record Result
    {
        public sealed record Success(Guid UserSessionId) : Result;

        public sealed record Failure(string Reason) : Result;
    }
}