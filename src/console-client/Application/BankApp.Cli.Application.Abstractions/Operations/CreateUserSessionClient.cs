using BankApp.Cli.Application.Models;

namespace BankApp.Cli.Application.Abstractions.Operations;

public sealed class CreateUserSessionClient
{
    public readonly record struct Request(long AccountId, string PinCode);

    public abstract record Result
    {
        public sealed record Success(UserSession CreatedSession) : Result;

        public sealed record Failure(string Reason) : Result;
    }
}