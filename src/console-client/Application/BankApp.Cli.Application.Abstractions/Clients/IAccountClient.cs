using BankApp.Cli.Application.Abstractions.Operations;

namespace BankApp.Cli.Application.Abstractions.Clients;

public interface IAccountClient
{
    Task<GetBalanceRequest.Result> GetBalanceAsync(
        GetBalanceRequest.Request request,
        CancellationToken cancellationToken);

    Task<CreateAccountRequest.Result> CreateNewAccountAsync(
        CreateAccountRequest.Request request,
        CancellationToken cancellationToken);

    Task<DepositMoneyRequest.Result> DepositMoneyAsync(
        DepositMoneyRequest.Request request,
        CancellationToken cancellationToken);

    Task<WithdrawMoneyRequest.Result> WithdrawMoneyAsync(
        WithdrawMoneyRequest.Request request,
        CancellationToken cancellationToken);

    Task<GetHistoryRequest.Result> GetHistoryAsync(
        GetHistoryRequest.Request request,
        CancellationToken cancellationToken);
}