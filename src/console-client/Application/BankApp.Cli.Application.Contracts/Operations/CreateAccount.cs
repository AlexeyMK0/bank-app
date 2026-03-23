using BankApp.Cli.Application.Contracts.Model;

namespace BankApp.Cli.Application.Contracts.Operations;

public class CreateAccount
{
    public sealed record Request(string PinCode);

    public abstract record Result
    {
        public sealed record Success(AccountDto Account) : Result;

        public sealed record Failure(string Reason) : Result;
    }
}