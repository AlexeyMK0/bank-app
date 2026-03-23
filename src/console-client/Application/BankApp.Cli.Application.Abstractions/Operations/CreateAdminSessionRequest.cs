namespace BankApp.Cli.Application.Abstractions.Operations;

public sealed class CreateAdminSessionRequest
{
    public readonly record struct Request(string SystemPassword);

    public abstract record Result
    {
        public sealed record Success(Guid CreatedSessionId) : Result;

        public sealed record Failure(string Reason) : Result;
    }
}