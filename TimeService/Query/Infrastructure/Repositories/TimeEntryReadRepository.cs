using MongoDB.Driver;
using TimeService.Query.Domain;
using TimeService.Query.Domain.Repositories;

namespace TimeService.Query.Infrastructure.Repositories;

public class TimeEntryReadRepository : ITimeEntryReadRepository
{
    private readonly ReadDbContext _readDbContext;

    public TimeEntryReadRepository(ReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<TimeEntryReadModel?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _readDbContext.TimeEntries.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

    public async Task<long> CountAsync(CancellationToken ct)
        => await _readDbContext.TimeEntries.CountDocumentsAsync(FilterDefinition<TimeEntryReadModel>.Empty, cancellationToken: ct);

    public async Task<IReadOnlyList<TimeEntryReadModel>> GetPagedAsync(int skip, int take, CancellationToken ct)
        => await _readDbContext.TimeEntries
            .Find(_ => true)
            .SortByDescending(x => x.WorkDate)
            .ThenByDescending(x => x.CreatedAt)
            .Skip(skip)
            .Limit(take)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<TimeEntryReadModel>> GetByEmployeeAsync(Guid employeeId, DateOnly? from, DateOnly? to, CancellationToken ct)
    {
        var filter = BuildDateFilter(Builders<TimeEntryReadModel>.Filter.Eq(x => x.EmployeeId, employeeId), from, to);
        return await _readDbContext.TimeEntries.Find(filter).SortByDescending(x => x.WorkDate).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<TimeEntryReadModel>> GetByAccountAsync(Guid accountId, DateOnly? from, DateOnly? to, CancellationToken ct)
    {
        var filter = BuildDateFilter(Builders<TimeEntryReadModel>.Filter.Eq(x => x.AccountId, accountId), from, to);
        return await _readDbContext.TimeEntries.Find(filter).SortByDescending(x => x.WorkDate).ToListAsync(ct);
    }

    private static FilterDefinition<TimeEntryReadModel> BuildDateFilter(FilterDefinition<TimeEntryReadModel> baseFilter, DateOnly? from, DateOnly? to)
    {
        var builder = Builders<TimeEntryReadModel>.Filter;
        var filter = baseFilter;

        if (from.HasValue)
        {
            filter &= builder.Gte(x => x.WorkDate, from.Value.ToDateTime(TimeOnly.MinValue));
        }

        if (to.HasValue)
        {
            filter &= builder.Lte(x => x.WorkDate, to.Value.ToDateTime(TimeOnly.MaxValue));
        }

        return filter;
    }
}
