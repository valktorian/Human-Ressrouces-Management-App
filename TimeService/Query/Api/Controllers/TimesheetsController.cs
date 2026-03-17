using Infrastructure.Api.Common;
using Infrastructure.Api.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimeService.Query.Domain;
using TimeService.Query.Domain.Repositories;

namespace TimeService.Query.Api.Controllers;

[ApiController]
[Route("api/timesheets")]
[Authorize]
public class TimesheetsController : ControllerBase
{
    private readonly ITimesheetReadRepository _repository;

    public TimesheetsController(ITimesheetReadRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var item = await _repository.GetByIdAsync(id, ct);
        return item is null
            ? NotFound(BaseResponse<object>.Fail("Timesheet not found."))
            : Ok(BaseResponse<TimesheetReadModel>.Ok(item));
    }

    [HttpGet]
    [Authorize(Roles = RoleConstants.ManagerOrHrAdmin)]
    public async Task<IActionResult> GetAll([FromQuery] PaginationRequest pagination, CancellationToken ct)
    {
        var totalCount = await _repository.CountAsync(ct);
        var items = await _repository.GetPagedAsync(pagination.Skip, pagination.NormalizedPageSize, ct);
        var result = PagedResult<TimesheetReadModel>.Create(items, pagination.NormalizedPageNumber, pagination.NormalizedPageSize, totalCount);
        return Ok(BaseResponse<PagedResult<TimesheetReadModel>>.Ok(result));
    }

    [HttpGet("by-employee/{employeeId:guid}")]
    [Authorize(Roles = RoleConstants.ManagerOrHrAdmin)]
    public async Task<IActionResult> GetByEmployee(Guid employeeId, [FromQuery] DateOnly? periodStart, [FromQuery] DateOnly? periodEnd, CancellationToken ct)
        => Ok(BaseResponse<IReadOnlyList<TimesheetReadModel>>.Ok(await _repository.GetByEmployeeAsync(employeeId, periodStart, periodEnd, ct)));

    [HttpGet("pending-approval")]
    [Authorize(Roles = RoleConstants.ManagerOrHrAdmin)]
    public async Task<IActionResult> GetPendingApproval([FromQuery] PaginationRequest pagination, CancellationToken ct)
    {
        var totalCount = await _repository.CountPendingApprovalAsync(ct);
        var items = await _repository.GetPendingApprovalAsync(pagination.Skip, pagination.NormalizedPageSize, ct);
        var result = PagedResult<TimesheetReadModel>.Create(items, pagination.NormalizedPageNumber, pagination.NormalizedPageSize, totalCount);
        return Ok(BaseResponse<PagedResult<TimesheetReadModel>>.Ok(result));
    }

    [HttpGet("self")]
    public async Task<IActionResult> GetSelf([FromQuery] DateOnly? periodStart, [FromQuery] DateOnly? periodEnd, CancellationToken ct)
        => Ok(BaseResponse<IReadOnlyList<TimesheetReadModel>>.Ok(await _repository.GetByAccountAsync(GetCurrentAccountId(), periodStart, periodEnd, ct)));

    private Guid GetCurrentAccountId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(value, out var id) ? id : Guid.Empty;
    }
}
