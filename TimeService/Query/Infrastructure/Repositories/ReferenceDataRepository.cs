using TimeService.Query.Domain;
using TimeService.Query.Domain.Repositories;

namespace TimeService.Query.Infrastructure.Repositories;

public class ReferenceDataRepository : IReferenceDataRepository
{
    public Task<IReadOnlyList<HolidayReadModel>> GetHolidaysAsync(int year, string country, CancellationToken ct)
    {
        IReadOnlyList<HolidayReadModel> items =
        [
            new HolidayReadModel { Date = new DateTime(year, 1, 1), Name = "New Year", Country = country },
            new HolidayReadModel { Date = new DateTime(year, 5, 1), Name = "Labour Day", Country = country }
        ];

        return Task.FromResult(items);
    }

    public Task<IReadOnlyList<LeaveTypeReadModel>> GetLeaveTypesAsync(CancellationToken ct)
    {
        IReadOnlyList<LeaveTypeReadModel> items =
        [
            new LeaveTypeReadModel { Code = "Annual", Name = "Annual Leave", IsPaid = true },
            new LeaveTypeReadModel { Code = "Sick", Name = "Sick Leave", IsPaid = true },
            new LeaveTypeReadModel { Code = "Unpaid", Name = "Unpaid Leave", IsPaid = false }
        ];

        return Task.FromResult(items);
    }
}
