using Infrastructure.Api.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Api.Messaging;
using TimeService.Command.Application.Commands;
using TimeService.Command.Application.DTOs;

namespace TimeService.Command.Api.Controllers;

[ApiController]
[Route("api/time-entries")]
[Authorize]
public class TimeEntriesController : ControllerBase
{
    private readonly ICommandDispatcher _dispatcher;

    public TimeEntriesController(ICommandDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost]
    [Authorize(Roles = RoleConstants.EmployeeManagerOrHrAdmin)]
    public async Task<IActionResult> Create([FromBody] CreateTimeEntryCommand command, CancellationToken ct)
        => Ok(await _dispatcher.SendAsync<CreateTimeEntryCommand, CommandAcceptedResponse>(command, ct));

    [HttpPut("{id:guid}")]
    [Authorize(Roles = RoleConstants.EmployeeManagerOrHrAdmin)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTimeEntryCommand command, CancellationToken ct)
        => Ok(await _dispatcher.SendAsync<UpdateTimeEntryCommand, CommandAcceptedResponse>(command with { Id = id }, ct));

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleConstants.HrAdmin)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _dispatcher.SendAsync<DeleteTimeEntryCommand, CommandAcceptedResponse>(new DeleteTimeEntryCommand(id), ct);
        return NoContent();
    }
}
