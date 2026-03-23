using BankApp.Cli.Application.Contracts.Model;

namespace BankApp.Cli.Application.Contracts.Operations;

public class GetHistory
{
    public sealed record Request(int EntriesCount);

    public abstract record Result
    {
        public sealed record Success(IEnumerable<OperationRecordDto> Operations) : Result;

        public sealed record Failure(string Reason) : Result;
    }
}