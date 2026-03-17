using Infrastructure.Api.Authentication;
using Infrastructure.Api.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfileService.Command.Application.Commands;
using ProfileService.Command.Application.DTOs;

namespace ProfileService.Command.Api.Controllers;

[ApiController]
[Route("api/profiles")]
[Authorize]
public class ProfileController : ControllerBase
{
    private const string HrRoles = "HRAdmin,HRManager";
    private readonly ICommandDispatcher _dispatcher;
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public ProfileController(ICommandDispatcher dispatcher, ICurrentUserAccessor currentUserAccessor)
    {
        _dispatcher = dispatcher;
        _currentUserAccessor = currentUserAccessor;
    }

    [HttpPost]
    [Authorize(Roles = HrRoles)]
    public async Task<IActionResult> Create([FromBody] CreateProfileCommand command, CancellationToken ct)
    {
        var response = await _dispatcher.SendAsync<CreateProfileCommand, ProfileResponse>(command, ct);
        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = HrRoles)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProfileCommand command, CancellationToken ct)
    {
        var response = await _dispatcher.SendAsync<UpdateProfileCommand, ProfileResponse>(command with { ProfileId = id }, ct);
        return Ok(response);
    }

    [HttpPatch("{id:guid}/employment")]
    [Authorize(Roles = HrRoles)]
    public async Task<IActionResult> UpdateEmployment(Guid id, [FromBody] UpdateProfileEmploymentCommand command, CancellationToken ct)
    {
        var response = await _dispatcher.SendAsync<UpdateProfileEmploymentCommand, ProfileResponse>(command with { ProfileId = id }, ct);
        return Ok(response);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = HrRoles)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateProfileStatusCommand command, CancellationToken ct)
    {
        var response = await _dispatcher.SendAsync<UpdateProfileStatusCommand, ProfileResponse>(command with { ProfileId = id }, ct);
        return Ok(response);
    }

    [HttpPost("{id:guid}/link-account")]
    [Authorize(Roles = HrRoles)]
    public async Task<IActionResult> LinkAccount(Guid id, [FromBody] LinkProfileAccountCommand command, CancellationToken ct)
    {
        var response = await _dispatcher.SendAsync<LinkProfileAccountCommand, ProfileResponse>(command with { ProfileId = id }, ct);
        return Ok(response);
    }

    [HttpPatch("self/personal-info")]
    public async Task<IActionResult> UpdateSelfPersonalInfo([FromBody] UpdateSelfPersonalInfoCommand command, CancellationToken ct)
    {
        var response = await _dispatcher.SendAsync<UpdateSelfPersonalInfoCommand, ProfileResponse>(
            command with { AccountId = _currentUserAccessor.GetRequiredAccountId() },
            ct);

        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = HrRoles)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _dispatcher.SendAsync<DeleteProfileCommand, bool>(new DeleteProfileCommand(id), ct);
        return NoContent();
    }
}
