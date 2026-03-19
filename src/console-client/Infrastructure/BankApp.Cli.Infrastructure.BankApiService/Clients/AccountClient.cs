using BankApp.Cli.Application.Abstractions.Clients;
using BankApp.Cli.Application.Abstractions.Operations;
using BankApp.Cli.Application.Models;
using BankApp.Cli.Infrastructure.BankApiService.Models;
using BankApp.Cli.Infrastructure.BankApiService.RefitClients;
using Refit;

namespace BankApp.Cli.Infrastructure.BankApiService.Clients;

public class AccountClient : IAccountClient
{
    private readonly IRefitAccountClient _accountClient;

    public AccountClient(IRefitAccountClient accountClient)
    {
        _accountClient = accountClient;
    }

    public async Task<GetBalanceClient.Result> GetBalanceAsync(
        GetBalanceClient.Request request,
        CancellationToken cancellationToken)
    {
        IApiResponse<decimal> apiResponse =
            await _accountClient.CheckAccountBalanceAsync(request.SessionId, cancellationToken);
        if (!apiResponse.IsSuccessful)
        {
            return new GetBalanceClient.Result.Failure(apiResponse.ReasonPhrase ?? apiResponse.Error.Message);
        }

        return new GetBalanceClient.Result.Success(apiResponse.Content);
    }

    public async Task<CreateAccountClient.Result> CreateNewAccountAsync(
        CreateAccountClient.Request request,
        CancellationToken cancellationToken)
    {
        IApiResponse<AccountDto> apiResponse =
            await _accountClient.CreateNewAccountAsync(
                new CreateAccountRequest(request.AdminSessionId, request.PinCode),
                cancellationToken);
        if (!apiResponse.IsSuccessful)
        {
            return new CreateAccountClient.Result.Failure(apiResponse.ReasonPhrase ?? apiResponse.Error.Message);
        }

        AccountDto content = apiResponse.Content;
        return new CreateAccountClient.Result.Success(new Account(content.AccountId, content.Balance));
    }

    public async Task<DepositMoneyClient.Result> DepositMoneyAsync(
        DepositMoneyClient.Request request,
        CancellationToken cancellationToken)
    {
        IApiResponse<AccountDto> apiResponse =
            await _accountClient.DepositMoneyAsync(
                new DepositMoneyRequest(request.Amount, request.SessionId),
                cancellationToken);
        if (!apiResponse.IsSuccessful)
        {
            return new DepositMoneyClient.Result.Failure(apiResponse.ReasonPhrase ?? apiResponse.Error.Message);
        }

        return new DepositMoneyClient.Result.Success(apiResponse.Content.Balance);
    }

    public async Task<WithdrawMoneyClient.Result> WithdrawMoneyAsync(
        WithdrawMoneyClient.Request request,
        CancellationToken cancellationToken)
    {
        IApiResponse<AccountDto> apiResponse =
            await _accountClient.WithdrawMoneyAsync(
                new WithdrawMoneyRequest(request.Amount, request.SessionId),
                cancellationToken);
        if (!apiResponse.IsSuccessful)
        {
            return new WithdrawMoneyClient.Result.Failure(apiResponse.ReasonPhrase ?? apiResponse.Error.Message);
        }

        return new WithdrawMoneyClient.Result.Success(apiResponse.Content.Balance);
    }

    public async Task<GetHistoryClient.Result> GetHistoryAsync(
        GetHistoryClient.Request request,
        CancellationToken cancellationToken)
    {
        IApiResponse<GetHistoryReponse> apiResponse =
            await _accountClient.GetOperationHistoryAsync(
                null,
                request.EntriesCount,
                request.SessionId,
                cancellationToken);

        if (!apiResponse.IsSuccessful)
        {
            return new GetHistoryClient.Result.Failure(apiResponse.ReasonPhrase ?? apiResponse.Error.Message);
        }

        return new GetHistoryClient.Result.Success(apiResponse.Content.Entries);
    }
}