using Microsoft.AspNetCore.Mvc;

namespace Lab1.Presentation.Http;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    /*private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet("balance")]
    public ActionResult<BalanceDto> CheckAccountBalance(Guid sessionId)
    {
        var request = new CheckBalance.Request(sessionId);
        CheckBalance.Response response = _accountService.CheckBalance(request);
        return response switch
        {
            CheckBalance.Response.Success success => Ok(success.Balance),
            CheckBalance.Response.Failure failure => BadRequest(failure.Message),
            _ => throw new UnreachableException(),
        };
    }

    [HttpPost("create")]
    public ActionResult<AccountDto> CreateNewAccount([FromBody] CreateAccountRequest httpRequest)
    {
        var request = new CreateAccount.Request(httpRequest.PinCode, httpRequest.SessionId);
        CreateAccount.Response response = _accountService.CreateAccount(request);
        return response switch
        {
            CreateAccount.Response.Success success => Ok(success.AccountDto),
            CreateAccount.Response.Failure failure => BadRequest(failure.Message),
            _ => throw new UnreachableException(),
        };
    }

    [HttpPost("deposit")]
    public ActionResult<AccountDto> DepositSum([FromBody] DepositMoneyRequest httpRequest)
    {
        var request = new DepositMoney.Request(httpRequest.Amount, httpRequest.SessionId);
        DepositMoney.Response response = _accountService.DepositMoney(request);
        return response switch
        {
            DepositMoney.Response.Success success => Ok(success.AccountDto),
            DepositMoney.Response.Failure failure => BadRequest(failure.Message),
            _ => throw new UnreachableException(),
        };
    }

    [HttpPost("withdraw")]
    public ActionResult<AccountDto> WithdrawSum([FromBody] WithdrawMoneyRequest httpRequest)
    {
        var request = new WithdrawMoney.Request(httpRequest.Amount, httpRequest.SessionId);
        WithdrawMoney.Response response = _accountService.WithdrawMoney(request);
        return response switch
        {
            WithdrawMoney.Response.Success success => Ok(success.AccountDto),
            WithdrawMoney.Response.Failure failure => BadRequest(failure.Message),
            _ => throw new UnreachableException(),
        };
    }

    [HttpGet("history")]
    public ActionResult<HistoryDto> CheckHistory(Guid sessionId)
    {
        var request = new OperationHistory.Request(sessionId);
        OperationHistory.Response response = _accountService.OperationHistory(request);
        return response switch
        {
            OperationHistory.Response.Success success => Ok(success.HistoryDto),
            OperationHistory.Response.Failure failure => BadRequest(failure.Message),
            _ => throw new UnreachableException(),
        };
    }*/
}