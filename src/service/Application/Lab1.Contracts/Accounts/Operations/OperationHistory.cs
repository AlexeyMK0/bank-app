using Contracts.Accounts.Model;

namespace Contracts.Accounts.Operations;

public static class OperationHistory
{
    public sealed record Request(Guid SessionId, string? KeyCursor, int PageSize);

    public abstract record Response
    {
        public sealed record Success(HistoryDto HistoryDto, string KeyCursor) : Response;

        public sealed record Failure(string Message) : Response;
    }
}