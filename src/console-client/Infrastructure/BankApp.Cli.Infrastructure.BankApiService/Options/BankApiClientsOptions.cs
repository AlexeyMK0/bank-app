using System.ComponentModel.DataAnnotations;

namespace BankApp.Cli.Infrastructure.BankApiService.Options;

public class BankApiClientsOptions
{
    public required Uri ConnectionUri { get; set; }

    [Range(1, 100)]
    public required int DefaultPageSize { get; set; }
}