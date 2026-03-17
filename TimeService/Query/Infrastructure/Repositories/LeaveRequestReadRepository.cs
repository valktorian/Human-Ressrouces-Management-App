using MongoDB.Driver;
using TimeService.Query.Domain;
using TimeService.Query.Domain.Repositories;

namespace TimeService.Query.Infrastructure.Repositories;

public class LeaveRequestReadRepository : ILeaveRequestReadRepository
{
    private readonly ReadDbContext _readDbContext;

    public LeaveRequestReadRepository(ReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<LeaveRequestReadModel?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _readDbContext.LeaveRequests.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

    public async Task<long> CountAsync(CancellationToken ct)
        => await _readDbContext.LeaveRequests.CountDocumentsAsync(FilterDefinition<LeaveRequestReadModel>.Empty, cancellationToken: ct);

    public async Task<long> CountPendingApprovalAsync(CancellationToken ct)
        => await _readDbContext.LeaveRequests.CountDocumentsAsync(x => x.Status == "Submitted", cancellationToken: ct);

    public async Task<IReadOnlyList<LeaveRequestReadModel>> GetPagedAsync(int skip, int take, CancellationToken ct)
        => await _readDbContext.LeaveRequests
            .Find(_ => true)
            .SortByDescending(x => x.CreatedAt)
            .Skip(skip)
            .Limit(take)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<LeaveRequestReadModel>> GetPendingApprovalAsync(int skip, int take, CancellationToken ct)
        => await _readDbContext.LeaveRequests
            .Find(x => x.Status == "Submitted")
            .SortByDescending(x => x.SubmittedAt)
            .Skip(skip)
            .Limit(take)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<LeaveRequestReadModel>> GetByEmployeeAsync(Guid employeeId, CancellationToken ct)
        => await _readDbContext.LeaveRequests
            .Find(x => x.EmployeeId == employeeId)
            .SortByDescending(x => x.StartDate)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<LeaveRequestReadModel>> GetByAccountAsync(Guid accountId, CancellationToken ct)
        => await _readDbContext.LeaveRequests
            .Find(x => x.AccountId == accountId)
            .SortByDescending(x => x.StartDate)
            .ToListAsync(ct);
}
