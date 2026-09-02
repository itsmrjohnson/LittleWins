using LittleWins.Application.UseCases.Activities.CreateActivity;
using Microsoft.AspNetCore.Mvc;

namespace LittleWins.Api.Controllers;

[ApiController]
[Route("api/activities")]
public sealed class ActivitiesController : ControllerBase
{
    private readonly CreateActivityHandler _handler;

    public ActivitiesController(CreateActivityHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    public async Task<ActionResult<CreateActivityResult>> Create(
        CreateActivityCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _handler.HandleAsync(
            command,
            cancellationToken);

        return Created(
            $"/api/activities/{result.ActivityId}",
            result);
    }
}