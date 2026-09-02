using LittleWins.Application.UseCases.Families.CreateFamily;
using Microsoft.AspNetCore.Mvc;

namespace LittleWins.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class FamiliesController : ControllerBase
{
    private readonly CreateFamilyHandler _handler;

    public FamiliesController(CreateFamilyHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    public async Task<ActionResult<CreateFamilyResult>> Create(
        CreateFamilyCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _handler.HandleAsync(
            command,
            cancellationToken);

        return Created(
            $"/api/families/{result.FamilyId}",
            result);
    }
}