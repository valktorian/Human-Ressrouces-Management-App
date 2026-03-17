namespace TimeService.Query.Domain.Repositories;

public interface ITimesheetReadRepository
{
    Task<TimesheetReadModel?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<long> CountAsync(CancellationToken ct);
    Task<long> CountPendingApprovalAsync(CancellationToken ct);
    Task<IReadOnlyList<TimesheetReadModel>> GetPagedAsync(int skip, int take, CancellationToken ct);
    Task<IReadOnlyList<TimesheetReadModel>> GetPendingApprovalAsync(int skip, int take, CancellationToken ct);
    Task<IReadOnlyList<TimesheetReadModel>> GetByEmployeeAsync(Guid employeeId, DateOnly? periodStart, DateOnly? periodEnd, CancellationToken ct);
    Task<IReadOnlyList<TimesheetReadModel>> GetByAccountAsync(Guid accountId, DateOnly? periodStart, DateOnly? periodEnd, CancellationToken ct);
}
