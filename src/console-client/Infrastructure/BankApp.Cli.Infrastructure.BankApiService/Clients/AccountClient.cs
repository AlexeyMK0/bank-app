using BankApp.Cli.Application.Abstractions.Clients;
using BankApp.Cli.Application.Abstractions.Operations;
using BankApp.Cli.Application.Models;
using BankApp.Cli.Infrastructure.BankApiService.Models;
using BankApp.Cli.Infrastructure.BankApiService.Options;
using BankApp.Cli.Infrastructure.BankApiService.RefitClients;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refit;

namespace BankApp.Cli.Infrastructure.BankApiService.Clients;

public class AccountClient : IAccountClient
{
    private readonly int _defaultPageSize;
    private readonly IRefitAccountClient _accountClient;

    private readonly ILogger _logger =
        LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("AccountClient logger");

    public AccountClient(IRefitAccountClient accountClient, IOptions<BankApiClientsOptions> options)
    {
        _accountClient = accountClient;
        _defaultPageSize = options.Value.DefaultPageSize;
    }

    public async Task<GetBalanceClient.Result> GetBalanceAsync(
        GetBalanceClient.Request request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting balance of selected account");
        IApiResponse<decimal> apiResponse =
            await _accountClient.CheckAccountBalanceAsync(request.SessionId, cancellationToken);
        if (!apiResponse.IsSuccessful)
        {
            _logger.LogInformation("Operation failed");
            _logger.LogInformation(string.Empty + apiResponse);
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
        _logger.LogInformation("Getting history of account in client");

        var entries = new List<OperationRecord>();
        string? pageToken = null;

        do
        {
            _logger.LogInformation("Request to refit client");
            int pageSize = Math.Min(_defaultPageSize, request.EntriesCount - entries.Count);
            IApiResponse<GetHistoryReponse> apiResponse =
                await _accountClient.GetOperationHistoryAsync(
                    pageToken,
                    pageSize,
                    request.SessionId,
                    cancellationToken);
            _logger.LogInformation("Got response from refit client");

            if (!apiResponse.IsSuccessful)
            {
                _logger.LogInformation("Operation failed" + apiResponse.ReasonPhrase);
                return new GetHistoryClient.Result.Failure(apiResponse.ReasonPhrase ?? apiResponse.Error.Message);
            }

            _logger.LogInformation("apiResponse is successful");
            pageToken = apiResponse.Content.PageToken;
            entries.AddRange(apiResponse.Content.Operations
                .Select(entry => new OperationRecord(entry.OperationType, entry.Time, entry.AccountId, entry.SessionId)));
            _logger.LogInformation("added new range to entries");
        }
        while (pageToken is not null && entries.Count < request.EntriesCount);

        _logger.LogInformation("Exit GetHistoryAsync");

        return new GetHistoryClient.Result.Success(entries);
    }
}