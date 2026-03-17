namespace TimeService.Query.Domain.Repositories;

public interface ILeaveBalanceReadRepository
{
    Task<IReadOnlyList<LeaveBalanceReadModel>> GetByEmployeeAsync(Guid employeeId, CancellationToken ct);
    Task<IReadOnlyList<LeaveBalanceReadModel>> GetByAccountAsync(Guid accountId, CancellationToken ct);
}
