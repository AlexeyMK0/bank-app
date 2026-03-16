namespace Lab1.Domain.Sessions;

// TODO: add id processing
public sealed record AdminSession(long? Id, SessionId SessionGuid);