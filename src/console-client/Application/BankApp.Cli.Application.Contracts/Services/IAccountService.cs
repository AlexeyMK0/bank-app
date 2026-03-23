using BankApp.Cli.Application.Contracts.Operations;

namespace BankApp.Cli.Application.Contracts.Services;

public interface IAccountService
{
    Task<CreateAccount.Result> CreateAccountAsync(CreateAccount.Request request, CancellationToken cancellationToken);

    Task<GetBalance.Result> GetBalanceAsync(GetBalance.Request request, CancellationToken cancellationToken);

    Task<WithdrawMoney.Result> WithdrawMoneyAsync(WithdrawMoney.Request request, CancellationToken cancellationToken);

    Task<DepositMoney.Result> DepositMoneyAsync(DepositMoney.Request request, CancellationToken cancellationToken);

    Task<GetHistory.Result> GetHistoryAsync(GetHistory.Request request, CancellationToken cancellationToken);
}