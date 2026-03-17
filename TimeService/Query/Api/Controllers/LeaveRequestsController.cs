using Infrastructure.Api.Common;
using Infrastructure.Api.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimeService.Query.Domain;
using TimeService.Query.Domain.Repositories;

namespace TimeService.Query.Api.Controllers;

[ApiController]
[Route("api/leave-requests")]
[Authorize]
public class LeaveRequestsController : ControllerBase
{
    private readonly ILeaveRequestReadRepository _repository;

    public LeaveRequestsController(ILeaveRequestReadRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var item = await _repository.GetByIdAsync(id, ct);
        return item is null
            ? NotFound(BaseResponse<object>.Fail("Leave request not found."))
            : Ok(BaseResponse<LeaveRequestReadModel>.Ok(item));
    }

    [HttpGet]
    [Authorize(Roles = RoleConstants.ManagerOrHrAdmin)]
    public async Task<IActionResult> GetAll([FromQuery] PaginationRequest pagination, CancellationToken ct)
    {
        var totalCount = await _repository.CountAsync(ct);
        var items = await _repository.GetPagedAsync(pagination.Skip, pagination.NormalizedPageSize, ct);
        var result = PagedResult<LeaveRequestReadModel>.Create(items, pagination.NormalizedPageNumber, pagination.NormalizedPageSize, totalCount);
        return Ok(BaseResponse<PagedResult<LeaveRequestReadModel>>.Ok(result));
    }

    [HttpGet("by-employee/{employeeId:guid}")]
    [Authorize(Roles = RoleConstants.ManagerOrHrAdmin)]
    public async Task<IActionResult> GetByEmployee(Guid employeeId, CancellationToken ct)
        => Ok(BaseResponse<IReadOnlyList<LeaveRequestReadModel>>.Ok(await _repository.GetByEmployeeAsync(employeeId, ct)));

    [HttpGet("pending-approval")]
    [Authorize(Roles = RoleConstants.ManagerOrHrAdmin)]
    public async Task<IActionResult> GetPendingApproval([FromQuery] PaginationRequest pagination, CancellationToken ct)
    {
        var totalCount = await _repository.CountPendingApprovalAsync(ct);
        var items = await _repository.GetPendingApprovalAsync(pagination.Skip, pagination.NormalizedPageSize, ct);
        var result = PagedResult<LeaveRequestReadModel>.Create(items, pagination.NormalizedPageNumber, pagination.NormalizedPageSize, totalCount);
        return Ok(BaseResponse<PagedResult<LeaveRequestReadModel>>.Ok(result));
    }

    [HttpGet("self")]
    public async Task<IActionResult> GetSelf(CancellationToken ct)
        => Ok(BaseResponse<IReadOnlyList<LeaveRequestReadModel>>.Ok(await _repository.GetByAccountAsync(GetCurrentAccountId(), ct)));

    private Guid GetCurrentAccountId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(value, out var id) ? id : Guid.Empty;
    }
}
