using Lab1.Domain.ValueObjects;

namespace Lab1.Domain.Accounts;

public sealed record Account
{
    public AccountId Id { get; }

    public PinCode PinCode { get; }

    public Money Balance { get; }

    public Account(AccountId id, PinCode pinCode, Money balance)
    {
        Id = id;
        PinCode = pinCode;
        Balance = balance;
    }
}