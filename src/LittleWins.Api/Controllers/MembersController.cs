using LittleWins.Application.UseCases.Members.AddMember;
using Microsoft.AspNetCore.Mvc;

namespace LittleWins.Api.Controllers;

[ApiController]
[Route("api/families/{familyId}/members")]
public sealed class MembersController : ControllerBase
{
    private readonly AddMemberHandler _handler;

    public MembersController(AddMemberHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    public async Task<ActionResult<AddMemberResult>> Create(
        Guid familyId,
        AddMemberCommand command,
        CancellationToken cancellationToken)
    {
        if (familyId != command.FamilyId)
        {
            return BadRequest(
                "Family ID in the URL does not match the Family ID in the request.");
        }

        var result = await _handler.HandleAsync(
            command,
            cancellationToken);

        return Created(
            $"/api/families/{result.FamilyId}/members/{result.MemberId}",
            result);
    }
}