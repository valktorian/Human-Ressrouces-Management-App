using Infrastructure.Api.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Api.Messaging;
using TimeService.Command.Application.Commands;
using TimeService.Command.Application.DTOs;

namespace TimeService.Command.Api.Controllers;

[ApiController]
[Route("api/leave-balances")]
[Authorize]
public class LeaveBalancesController : ControllerBase
{
    private readonly ICommandDispatcher _dispatcher;

    public LeaveBalancesController(ICommandDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost("{employeeId:guid}/adjust")]
    [Authorize(Roles = RoleConstants.HrAdmin)]
    public async Task<IActionResult> Adjust(Guid employeeId, [FromBody] AdjustLeaveBalanceCommand command, CancellationToken ct)
        => Ok(await _dispatcher.SendAsync<AdjustLeaveBalanceCommand, CommandAcceptedResponse>(command with { EmployeeId = employeeId }, ct));
}
