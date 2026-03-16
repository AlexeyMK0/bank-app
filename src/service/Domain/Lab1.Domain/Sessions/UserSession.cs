using Lab1.Domain.Accounts;

namespace Lab1.Domain.Sessions;

public sealed record UserSession(long? Id, SessionId SessionGuid, AccountId AccountId);