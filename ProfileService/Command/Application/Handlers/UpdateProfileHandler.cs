using Infrastructure.Api.Common;
using Infrastructure.Api.Messaging;
using Infrastructure.Api.Persistence;
using ProfileService.Command.Application.Abstractions;
using ProfileService.Command.Application.Commands;
using ProfileService.Command.Application.DTOs;

namespace ProfileService.Command.Application.Handlers;

public class UpdateProfileHandler : ICommandHandler<UpdateProfileCommand, ProfileResponse>
{
    private readonly IProfileRepository _profileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileHandler(IProfileRepository profileRepository, IUnitOfWork unitOfWork)
    {
        _profileRepository = profileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProfileResponse> HandleAsync(UpdateProfileCommand command, CancellationToken cancellationToken = default)
    {
        var profile = await _profileRepository.GetByIdAsync(command.ProfileId, cancellationToken)
            ?? throw ApiException.NotFound("Profile not found.");

        var employeeNumber = command.EmployeeNumber.Trim().ToUpperInvariant();
        var workEmail = command.WorkEmail.Trim().ToLowerInvariant();

        if (await _profileRepository.EmployeeNumberExistsAsync(profile.Id, employeeNumber, cancellationToken))
        {
            throw new ApiException("A profile with this employee number already exists.", 409);
        }

        if (await _profileRepository.WorkEmailExistsAsync(profile.Id, workEmail, cancellationToken))
        {
            throw new ApiException("A profile with this work email already exists.", 409);
        }

        profile.UpdateCore(employeeNumber, command.FirstName, command.LastName, workEmail, command.PersonalEmail, command.PhoneNumber, command.Address, command.DateOfBirth, command.JobTitle, command.Department, command.ManagerProfileId, command.EmploymentType, command.HireDate, command.OrganizationRole);
        await _profileRepository.UpdateAsync(profile, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return profile.ToResponse();
    }
}
