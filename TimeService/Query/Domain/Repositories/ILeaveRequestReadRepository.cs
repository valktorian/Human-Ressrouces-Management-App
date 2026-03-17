namespace TimeService.Query.Domain.Repositories;

public interface ILeaveRequestReadRepository
{
    Task<LeaveRequestReadModel?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<long> CountAsync(CancellationToken ct);
    Task<long> CountPendingApprovalAsync(CancellationToken ct);
    Task<IReadOnlyList<LeaveRequestReadModel>> GetPagedAsync(int skip, int take, CancellationToken ct);
    Task<IReadOnlyList<LeaveRequestReadModel>> GetPendingApprovalAsync(int skip, int take, CancellationToken ct);
    Task<IReadOnlyList<LeaveRequestReadModel>> GetByEmployeeAsync(Guid employeeId, CancellationToken ct);
    Task<IReadOnlyList<LeaveRequestReadModel>> GetByAccountAsync(Guid accountId, CancellationToken ct);
}
