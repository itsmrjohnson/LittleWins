using LittleWins.Application.UseCases.Completions.ApproveCompletion;
using LittleWins.Application.UseCases.Completions.CompleteActivity;
using Microsoft.AspNetCore.Mvc;

namespace LittleWins.Api.Controllers;

[ApiController]
public sealed class CompletionsController : ControllerBase
{
    private readonly CompleteActivityHandler _completeActivityHandler;
    private readonly ApproveCompletionHandler _approveCompletionHandler;

    public CompletionsController(
        CompleteActivityHandler completeActivityHandler,
        ApproveCompletionHandler approveCompletionHandler)
    {
        _completeActivityHandler = completeActivityHandler;
        _approveCompletionHandler = approveCompletionHandler;
    }

    [HttpPost("api/activities/{activityId}/complete")]
    public async Task<ActionResult<CompleteActivityResult>> Complete(
        Guid activityId,
        CompleteActivityCommand command,
        CancellationToken cancellationToken)
    {
        if (activityId != command.ActivityId)
        {
            return BadRequest(
                "Activity ID in the URL does not match the Activity ID in the request.");
        }

        var result = await _completeActivityHandler.HandleAsync(
            command,
            cancellationToken);

        return Created(
            $"/api/completions/{result.CompletionId}",
            result);
    }

    [HttpPost("api/completions/{completionId}/approve")]
    public async Task<ActionResult<ApproveCompletionResult>> Approve(
        Guid completionId,
        ApproveCompletionCommand command,
        CancellationToken cancellationToken)
    {
        if (completionId != command.CompletionId)
        {
            return BadRequest(
                "Completion ID in the URL does not match the Completion ID in the request.");
        }

        var result = await _approveCompletionHandler.HandleAsync(
            command,
            cancellationToken);

        return Ok(result);
    }
}