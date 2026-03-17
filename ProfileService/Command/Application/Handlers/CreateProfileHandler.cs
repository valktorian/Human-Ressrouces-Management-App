using Infrastructure.Api.Common;
using Infrastructure.Api.Messaging;
using Infrastructure.Api.Persistence;
using ProfileService.Command.Application.Abstractions;
using ProfileService.Command.Application.Commands;
using ProfileService.Command.Application.DTOs;
using ProfileService.Command.Domain;

namespace ProfileService.Command.Application.Handlers;

public class CreateProfileHandler : ICommandHandler<CreateProfileCommand, ProfileResponse>
{
    private readonly IProfileRepository _profileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProfileHandler(IProfileRepository profileRepository, IUnitOfWork unitOfWork)
    {
        _profileRepository = profileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProfileResponse> HandleAsync(CreateProfileCommand command, CancellationToken cancellationToken = default)
    {
        var employeeNumber = command.EmployeeNumber.Trim().ToUpperInvariant();
        var workEmail = command.WorkEmail.Trim().ToLowerInvariant();

        if (await _profileRepository.EmployeeNumberExistsAsync(employeeNumber, cancellationToken))
        {
            throw new ApiException("A profile with this employee number already exists.", 409);
        }

        if (await _profileRepository.WorkEmailExistsAsync(workEmail, cancellationToken))
        {
            throw new ApiException("A profile with this work email already exists.", 409);
        }

        var profile = Profile.Create(
            command.AccountId,
            employeeNumber,
            command.FirstName,
            command.LastName,
            workEmail,
            command.PersonalEmail,
            command.PhoneNumber,
            command.Address,
            command.DateOfBirth,
            command.JobTitle,
            command.Department,
            command.ManagerProfileId,
            command.EmploymentType,
            command.HireDate,
            command.OrganizationRole,
            command.EmploymentStatus);

        await _profileRepository.AddAsync(profile, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return profile.ToResponse();
    }
}
