using Infrastructure.Api.Common;
using Infrastructure.Api.Messaging;
using Infrastructure.Api.Persistence;
using ProfileService.Command.Application.Abstractions;
using ProfileService.Command.Application.Commands;
using ProfileService.Command.Application.DTOs;

namespace ProfileService.Command.Application.Handlers;

public class UpdateSelfPersonalInfoHandler : ICommandHandler<UpdateSelfPersonalInfoCommand, ProfileResponse>
{
    private readonly IProfileRepository _profileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSelfPersonalInfoHandler(IProfileRepository profileRepository, IUnitOfWork unitOfWork)
    {
        _profileRepository = profileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProfileResponse> HandleAsync(UpdateSelfPersonalInfoCommand command, CancellationToken cancellationToken = default)
    {
        var profile = await _profileRepository.GetByAccountIdAsync(command.AccountId, cancellationToken)
            ?? throw ApiException.NotFound("Profile not found for current account.");

        profile.UpdatePersonalInfo(command.PersonalEmail, command.PhoneNumber, command.Address, command.DateOfBirth);
        await _profileRepository.UpdateAsync(profile, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return profile.ToResponse();
    }
}
