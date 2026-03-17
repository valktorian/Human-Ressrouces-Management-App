using Infrastructure.Api.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeService.Query.Domain;
using TimeService.Query.Domain.Repositories;

namespace TimeService.Query.Api.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class ReferenceDataController : ControllerBase
{
    private readonly IReferenceDataRepository _repository;

    public ReferenceDataController(IReferenceDataRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("holidays")]
    public async Task<IActionResult> GetHolidays([FromQuery] int year, [FromQuery] string country = "MA", CancellationToken ct = default)
        => Ok(BaseResponse<IReadOnlyList<HolidayReadModel>>.Ok(await _repository.GetHolidaysAsync(year, country, ct)));

    [HttpGet("leave-types")]
    public async Task<IActionResult> GetLeaveTypes(CancellationToken ct)
        => Ok(BaseResponse<IReadOnlyList<LeaveTypeReadModel>>.Ok(await _repository.GetLeaveTypesAsync(ct)));
}
