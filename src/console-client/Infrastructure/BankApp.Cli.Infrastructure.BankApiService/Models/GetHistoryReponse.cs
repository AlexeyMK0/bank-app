using BankApp.Cli.Application.Models;

namespace BankApp.Cli.Infrastructure.BankApiService.Models;

public record GetHistoryReponse(IReadOnlyCollection<OperationRecord> Entries);