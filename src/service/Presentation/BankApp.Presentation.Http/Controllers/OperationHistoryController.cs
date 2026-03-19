using Contracts.OperationHistory;
using Lab1.Presentation.Http.Operations;
using Lab1.Presentation.Http.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace Lab1.Presentation.Http.Controllers;

[Route("api/operations/history")]
[ApiController]
public class OperationHistoryController : ControllerBase
{
    private readonly IOperationHistoryService _historyService;

    public OperationHistoryController(IOperationHistoryService historyService)
    {
        _historyService = historyService;
    }

    [HttpGet]
    public async Task<ActionResult<CheckHistoryResponse>> CheckHistory(
        [FromQuery] CheckHistoryRequest httpRequest,
        CancellationToken cancellationToken)
    {
        GetAccountOperations.PageToken? pageToken = httpRequest.PageToken is null
            ? null
            : JsonSerializer.Deserialize<GetAccountOperations.PageToken>(httpRequest.PageToken);

        var request = new GetAccountOperations.Request(httpRequest.SessionId, pageToken, httpRequest.PageSize);
        GetAccountOperations.Response response = await _historyService.GetAccountOperationsAsync(request, cancellationToken);
        return response switch
        {
            GetAccountOperations.Response.Success success => new CheckHistoryResponse(
                success.HistoryDto.Operations,
                success.KeyCursor is null ? null : JsonSerializer.Serialize(success.KeyCursor.Token)),
            GetAccountOperations.Response.Failure failure => BadRequest(failure.Message),
            _ => throw new UnreachableException(),
        };
    }
}