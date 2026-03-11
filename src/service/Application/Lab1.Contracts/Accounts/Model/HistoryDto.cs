namespace Contracts.Accounts.Model;

public record HistoryDto(IReadOnlyCollection<OperationRecordDto> Operations);