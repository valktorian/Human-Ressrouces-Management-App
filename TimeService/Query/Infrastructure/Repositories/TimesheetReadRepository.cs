using MongoDB.Driver;
using TimeService.Query.Domain;
using TimeService.Query.Domain.Repositories;

namespace TimeService.Query.Infrastructure.Repositories;

public class TimesheetReadRepository : ITimesheetReadRepository
{
    private readonly ReadDbContext _readDbContext;

    public TimesheetReadRepository(ReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<TimesheetReadModel?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _readDbContext.Timesheets.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

    public async Task<long> CountAsync(CancellationToken ct)
        => await _readDbContext.Timesheets.CountDocumentsAsync(FilterDefinition<TimesheetReadModel>.Empty, cancellationToken: ct);

    public async Task<long> CountPendingApprovalAsync(CancellationToken ct)
        => await _readDbContext.Timesheets.CountDocumentsAsync(x => x.Status == "Submitted", cancellationToken: ct);

    public async Task<IReadOnlyList<TimesheetReadModel>> GetPagedAsync(int skip, int take, CancellationToken ct)
        => await _readDbContext.Timesheets
            .Find(_ => true)
            .SortByDescending(x => x.CreatedAt)
            .Skip(skip)
            .Limit(take)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<TimesheetReadModel>> GetPendingApprovalAsync(int skip, int take, CancellationToken ct)
        => await _readDbContext.Timesheets
            .Find(x => x.Status == "Submitted")
            .SortByDescending(x => x.SubmittedAt)
            .Skip(skip)
            .Limit(take)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<TimesheetReadModel>> GetByEmployeeAsync(Guid employeeId, DateOnly? periodStart, DateOnly? periodEnd, CancellationToken ct)
    {
        var filter = BuildPeriodFilter(Builders<TimesheetReadModel>.Filter.Eq(x => x.EmployeeId, employeeId), periodStart, periodEnd);
        return await _readDbContext.Timesheets.Find(filter).SortByDescending(x => x.PeriodStart).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<TimesheetReadModel>> GetByAccountAsync(Guid accountId, DateOnly? periodStart, DateOnly? periodEnd, CancellationToken ct)
    {
        var filter = BuildPeriodFilter(Builders<TimesheetReadModel>.Filter.Eq(x => x.AccountId, accountId), periodStart, periodEnd);
        return await _readDbContext.Timesheets.Find(filter).SortByDescending(x => x.PeriodStart).ToListAsync(ct);
    }

    private static FilterDefinition<TimesheetReadModel> BuildPeriodFilter(FilterDefinition<TimesheetReadModel> baseFilter, DateOnly? periodStart, DateOnly? periodEnd)
    {
        var builder = Builders<TimesheetReadModel>.Filter;
        var filter = baseFilter;

        if (periodStart.HasValue)
        {
            filter &= builder.Gte(x => x.PeriodStart, periodStart.Value.ToDateTime(TimeOnly.MinValue));
        }

        if (periodEnd.HasValue)
        {
            filter &= builder.Lte(x => x.PeriodEnd, periodEnd.Value.ToDateTime(TimeOnly.MaxValue));
        }

        return filter;
    }
}
