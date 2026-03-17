using ProfileService.Command.Domain;

namespace ProfileService.Command.Application.Abstractions;

public interface IProfileRepository
{
    Task AddAsync(Profile profile, CancellationToken cancellationToken = default);

    Task<Profile?> GetByIdAsync(Guid profileId, CancellationToken cancellationToken = default);

    Task<Profile?> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);

    Task<bool> WorkEmailExistsAsync(string workEmail, CancellationToken cancellationToken = default);

    Task<bool> WorkEmailExistsAsync(Guid excludedProfileId, string workEmail, CancellationToken cancellationToken = default);

    Task<bool> EmployeeNumberExistsAsync(string employeeNumber, CancellationToken cancellationToken = default);

    Task<bool> EmployeeNumberExistsAsync(Guid excludedProfileId, string employeeNumber, CancellationToken cancellationToken = default);

    Task UpdateAsync(Profile profile, CancellationToken cancellationToken = default);

    Task DeleteAsync(Profile profile, CancellationToken cancellationToken = default);
}
