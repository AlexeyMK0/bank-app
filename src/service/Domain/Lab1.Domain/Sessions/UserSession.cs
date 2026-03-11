using Lab1.Domain.Accounts;

namespace Lab1.Domain.Sessions;

public sealed record UserSession(SessionId Id, AccountId AccountId);