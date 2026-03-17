namespace TimeService.Query.Domain.Repositories;

public interface ITimeEntryReadRepository
{
    Task<TimeEntryReadModel?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<long> CountAsync(CancellationToken ct);
    Task<IReadOnlyList<TimeEntryReadModel>> GetPagedAsync(int skip, int take, CancellationToken ct);
    Task<IReadOnlyList<TimeEntryReadModel>> GetByEmployeeAsync(Guid employeeId, DateOnly? from, DateOnly? to, CancellationToken ct);
    Task<IReadOnlyList<TimeEntryReadModel>> GetByAccountAsync(Guid accountId, DateOnly? from, DateOnly? to, CancellationToken ct);
}
