using MongoDB.Driver;
using TimeService.Query.Domain;
using TimeService.Query.Domain.Repositories;

namespace TimeService.Query.Infrastructure.Repositories;

public class LeaveBalanceReadRepository : ILeaveBalanceReadRepository
{
    private readonly ReadDbContext _readDbContext;

    public LeaveBalanceReadRepository(ReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<IReadOnlyList<LeaveBalanceReadModel>> GetByEmployeeAsync(Guid employeeId, CancellationToken ct)
        => await _readDbContext.LeaveBalances
            .Find(x => x.EmployeeId == employeeId)
            .SortBy(x => x.LeaveType)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<LeaveBalanceReadModel>> GetByAccountAsync(Guid accountId, CancellationToken ct)
        => await _readDbContext.LeaveBalances
            .Find(x => x.AccountId == accountId)
            .SortBy(x => x.LeaveType)
            .ToListAsync(ct);
}
