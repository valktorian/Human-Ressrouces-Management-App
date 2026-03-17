namespace TimeService.Query.Domain.Repositories;

public interface IReferenceDataRepository
{
    Task<IReadOnlyList<HolidayReadModel>> GetHolidaysAsync(int year, string country, CancellationToken ct);
    Task<IReadOnlyList<LeaveTypeReadModel>> GetLeaveTypesAsync(CancellationToken ct);
}
