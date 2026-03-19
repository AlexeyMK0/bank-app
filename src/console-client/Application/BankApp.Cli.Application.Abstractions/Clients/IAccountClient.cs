using BankApp.Cli.Application.Abstractions.Operations;

namespace BankApp.Cli.Application.Abstractions.Clients;

public interface IAccountClient
{
    Task<GetBalanceClient.Result> GetBalanceAsync(
        GetBalanceClient.Request request,
        CancellationToken cancellationToken);

    Task<CreateAccountClient.Result> CreateNewAccountAsync(
        CreateAccountClient.Request request,
        CancellationToken cancellationToken);

    Task<DepositMoneyClient.Result> DepositMoneyAsync(
        DepositMoneyClient.Request request,
        CancellationToken cancellationToken);

    Task<WithdrawMoneyClient.Result> WithdrawMoneyAsync(
        WithdrawMoneyClient.Request request,
        CancellationToken cancellationToken);

    Task<GetHistoryClient.Result> GetHistoryAsync(
        GetHistoryClient.Request request,
        CancellationToken cancellationToken);
}