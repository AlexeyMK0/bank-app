using BankApp.Cli.Application.Models;

namespace BankApp.Cli.Application.Abstractions.Operations;

public sealed class CreateAccountRequest
{
    public readonly record struct Request(string PinCode, Guid AdminSessionId);

    public abstract record Result
    {
        public sealed record Success(Account CreatedAccount) : Result;

        public sealed record Failure(string Reason) : Result;
    }
}