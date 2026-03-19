using Infrastructure.Api.Base;
using Infrastructure.Api.Messaging;
using Microsoft.EntityFrameworkCore;
using ProfileService.Command.Application.Abstractions;
using ProfileService.Command.Domain;
using ProfileService.Command.Domain.Events;
using ProfileService.Command.Infrastructure.Persistence;
using System.Text.Json;

namespace ProfileService.Command.Infrastructure.Repositories;

public class ProfileRepository : IProfileRepository
{
    private readonly ProfileCommandDbContext _dbContext;

    public ProfileRepository(ProfileCommandDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Profile profile, CancellationToken cancellationToken = default)
    {
        await _dbContext.Profiles.AddAsync(profile, cancellationToken);
        await AddSnapshotOutboxAsync(CreateCreatedEvent(profile), profile.Id, cancellationToken);
    }

    public Task<Profile?> GetByIdAsync(Guid profileId, CancellationToken cancellationToken = default)
        => _dbContext.Profiles.FirstOrDefaultAsync(x => x.Id == profileId, cancellationToken);

    public Task<Profile?> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
        => _dbContext.Profiles.FirstOrDefaultAsync(x => x.AccountId == accountId, cancellationToken);

    public Task<bool> WorkEmailExistsAsync(string workEmail, CancellationToken cancellationToken = default)
        => _dbContext.Profiles.AnyAsync(x => x.WorkEmail == workEmail, cancellationToken);

    public Task<bool> WorkEmailExistsAsync(Guid excludedProfileId, string workEmail, CancellationToken cancellationToken = default)
        => _dbContext.Profiles.AnyAsync(x => x.Id != excludedProfileId && x.WorkEmail == workEmail, cancellationToken);

    public Task<bool> EmployeeNumberExistsAsync(string employeeNumber, CancellationToken cancellationToken = default)
        => _dbContext.Profiles.AnyAsync(x => x.EmployeeNumber == employeeNumber, cancellationToken);

    public Task<bool> EmployeeNumberExistsAsync(Guid excludedProfileId, string employeeNumber, CancellationToken cancellationToken = default)
        => _dbContext.Profiles.AnyAsync(x => x.Id != excludedProfileId && x.EmployeeNumber == employeeNumber, cancellationToken);

    public Task UpdateAsync(Profile profile, CancellationToken cancellationToken = default)
        => AddSnapshotOutboxAsync(CreateUpdatedEvent(profile), profile.Id, cancellationToken);

    public async Task DeleteAsync(Profile profile, CancellationToken cancellationToken = default)
    {
        var deletedAt = DateTime.UtcNow;
        _dbContext.Profiles.Remove(profile);
        await AddOutboxMessageAsync(profile.Id, new ProfileDeletedEvent(profile.Id, deletedAt), new
        {
            ProfileId = profile.Id,
            DeletedAt = deletedAt
        }, cancellationToken);
    }

    private Task AddSnapshotOutboxAsync(BaseEvent evt, Guid profileId, CancellationToken cancellationToken)
        => AddOutboxMessageAsync(profileId, evt, evt, cancellationToken);

    private Task AddOutboxMessageAsync(Guid aggregateId, BaseEvent evt, object payload, CancellationToken cancellationToken)
    {
        return _dbContext.OutboxMessages.AddAsync(new OutboxMessage
        {
            AggregateType = nameof(Profile),
            AggregateId = aggregateId,
            EventType = evt.GetType().AssemblyQualifiedName!,
            Payload = JsonSerializer.Serialize(payload),
            OccurredAt = evt.OccurredAt,
        }, cancellationToken).AsTask();
    }

    private static ProfileCreatedEvent CreateCreatedEvent(Profile profile) => new(profile.Id, profile.AccountId, profile.EmployeeNumber, profile.FirstName, profile.LastName, profile.WorkEmail, profile.PersonalEmail, profile.PhoneNumber, profile.Address, profile.ProfilePictureUrl, profile.DateOfBirth, profile.JobTitle, profile.Department, profile.ManagerProfileId, profile.EmploymentType, profile.HireDate, profile.OrganizationRole, profile.EmploymentStatus, profile.CreatedAt, profile.UpdatedAt);

    private static ProfileUpdatedEvent CreateUpdatedEvent(Profile profile) => new(profile.Id, profile.AccountId, profile.EmployeeNumber, profile.FirstName, profile.LastName, profile.WorkEmail, profile.PersonalEmail, profile.PhoneNumber, profile.Address, profile.ProfilePictureUrl, profile.DateOfBirth, profile.JobTitle, profile.Department, profile.ManagerProfileId, profile.EmploymentType, profile.HireDate, profile.OrganizationRole, profile.EmploymentStatus, profile.CreatedAt, profile.UpdatedAt);
}
