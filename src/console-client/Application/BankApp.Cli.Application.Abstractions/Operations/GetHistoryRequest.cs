using BankApp.Cli.Application.Models;

namespace BankApp.Cli.Application.Abstractions.Operations;

public sealed class GetHistoryRequest
{
    public readonly record struct Request(Guid SessionId, int EntriesCount);

    public abstract record Result
    {
        public sealed record Success(IReadOnlyCollection<OperationRecord> Entries) : Result;

        public sealed record Failure(string Reason) : Result;
    }
}