using BankApp.Cli.Application.Abstractions.Clients;
using BankApp.Cli.Application.Abstractions.Operations;
using BankApp.Cli.Application.Models;
using BankApp.Cli.Infrastructure.BankApiService.Models;
using BankApp.Cli.Infrastructure.BankApiService.Options;
using BankApp.Cli.Infrastructure.BankApiService.RefitClients;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refit;
using CreateAccountRequest = BankApp.Cli.Application.Abstractions.Operations.CreateAccountRequest;
using DepositMoneyRequest = BankApp.Cli.Application.Abstractions.Operations.DepositMoneyRequest;
using WithdrawMoneyRequest = BankApp.Cli.Application.Abstractions.Operations.WithdrawMoneyRequest;

namespace BankApp.Cli.Infrastructure.BankApiService.Clients;

public class AccountClient : IAccountClient
{
    private readonly int _defaultPageSize;
    private readonly IRefitAccountClient _accountClient;
    private readonly ILogger<AccountClient> _logger;

    public AccountClient(IRefitAccountClient accountClient, IOptions<BankApiClientsOptions> options, ILogger<AccountClient> logger)
    {
        _accountClient = accountClient;
        _logger = logger;
        _defaultPageSize = options.Value.DefaultPageSize;
    }

    public async Task<GetBalanceRequest.Result> GetBalanceAsync(
        GetBalanceRequest.Request request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting balance of selected account");
        IApiResponse<decimal> apiResponse =
            await _accountClient.CheckAccountBalanceAsync(request.SessionId, cancellationToken);
        if (!apiResponse.IsSuccessful)
        {
            _logger.LogInformation("Operation failed");
            return new GetBalanceRequest.Result.Failure(apiResponse.Error.Content ?? apiResponse.Error.Message);
        }

        return new GetBalanceRequest.Result.Success(apiResponse.Content);
    }

    public async Task<CreateAccountRequest.Result> CreateNewAccountAsync(
        CreateAccountRequest.Request request,
        CancellationToken cancellationToken)
    {
        IApiResponse<AccountDto> apiResponse =
            await _accountClient.CreateNewAccountAsync(
                new CreateAccount.Request(request.AdminSessionId, request.PinCode),
                cancellationToken);
        if (!apiResponse.IsSuccessful)
        {
            return new CreateAccountRequest.Result.Failure(apiResponse.Error.Content ?? apiResponse.Error.Message);
        }

        AccountDto content = apiResponse.Content;
        return new CreateAccountRequest.Result.Success(new Account(content.AccountId, content.Balance));
    }

    public async Task<DepositMoneyRequest.Result> DepositMoneyAsync(
        DepositMoneyRequest.Request request,
        CancellationToken cancellationToken)
    {
        IApiResponse<AccountDto> apiResponse =
            await _accountClient.DepositMoneyAsync(
                new DepositMoney.Request(request.Amount, request.SessionId),
                cancellationToken);
        if (!apiResponse.IsSuccessful)
        {
            return new DepositMoneyRequest.Result.Failure(apiResponse.Error.Content ?? apiResponse.Error.Message);
        }

        return new DepositMoneyRequest.Result.Success(apiResponse.Content.Balance);
    }

    public async Task<WithdrawMoneyRequest.Result> WithdrawMoneyAsync(
        WithdrawMoneyRequest.Request request,
        CancellationToken cancellationToken)
    {
        var clientRequest = new WithdrawMoney.Request(
            request.Amount,
            request.SessionId);

        IApiResponse<AccountDto> apiResponse = await _accountClient
            .WithdrawMoneyAsync(clientRequest, cancellationToken);

        if (apiResponse.IsSuccessful is false)
        {
            return new WithdrawMoneyRequest.Result.Failure(apiResponse.Error.Content ?? apiResponse.Error.Message);
        }

        return new WithdrawMoneyRequest.Result.Success(apiResponse.Content.Balance);
    }

    public async Task<GetHistoryRequest.Result> GetHistoryAsync(
        GetHistoryRequest.Request request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting history of account in client");

        var entries = new List<OperationRecord>();
        string? pageToken = null;

        do
        {
            _logger.LogInformation("Request to refit client");
            int pageSize = Math.Min(_defaultPageSize, request.EntriesCount - entries.Count);
            IApiResponse<GetHistory.Response> apiResponse =
                await _accountClient.GetOperationHistoryAsync(
                    pageToken,
                    pageSize,
                    request.SessionId,
                    cancellationToken);
            _logger.LogInformation("Got response from refit client");

            if (!apiResponse.IsSuccessful)
            {
                _logger.LogInformation("Operation failed" + apiResponse.ReasonPhrase);
                return new GetHistoryRequest.Result.Failure(apiResponse.Error.Content ?? apiResponse.Error.Message);
            }

            _logger.LogInformation("apiResponse is successful");
            pageToken = apiResponse.Content.PageToken;
            entries.AddRange(apiResponse.Content.Operations
                .Select(entry => new OperationRecord(entry.OperationType, entry.Time, entry.AccountId, entry.SessionId)));
            _logger.LogInformation("added new range to entries");
        }
        while (pageToken is not null && entries.Count < request.EntriesCount);

        _logger.LogInformation("Exit GetHistoryAsync");

        return new GetHistoryRequest.Result.Success(entries);
    }
}