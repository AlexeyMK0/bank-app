using Lab1.Domain.Accounts;

namespace Lab1.Domain.Sessions;

// long? ID is a value for page-token
public sealed record UserSession(long? Id, SessionId SessionGuid, AccountId AccountId);