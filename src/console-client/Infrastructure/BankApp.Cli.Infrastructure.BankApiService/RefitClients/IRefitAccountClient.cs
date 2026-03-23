using BankApp.Cli.Infrastructure.BankApiService.Models;
using Refit;

namespace BankApp.Cli.Infrastructure.BankApiService.RefitClients;

public interface IRefitAccountClient
{
    [Get("/api/account/balance")]
    Task<IApiResponse<decimal>> CheckAccountBalanceAsync(
        [Query] Guid sessionId,
        CancellationToken cancellationToken);

    [Post("/api/account")]
    Task<IApiResponse<AccountDto>> CreateNewAccountAsync(
        [Body] CreateAccount.Request request,
        CancellationToken cancellationToken);

    [Post("/api/account/deposit")]
    Task<IApiResponse<AccountDto>> DepositMoneyAsync(
        [Body] DepositMoney.Request request,
        CancellationToken cancellationToken);

    [Post("/api/account/withdraw")]
    Task<IApiResponse<AccountDto>> WithdrawMoneyAsync(
        [Body] WithdrawMoney.Request request,
        CancellationToken cancellationToken);

    [Get("/api/operations/history")]
    Task<IApiResponse<GetHistory.Response>> GetOperationHistoryAsync(
        [Query] string? pageToken,
        [Query] int pageSize,
        [Query, AliasAs("sessionId")] Guid sessionId,
        CancellationToken cancellationToken);
}