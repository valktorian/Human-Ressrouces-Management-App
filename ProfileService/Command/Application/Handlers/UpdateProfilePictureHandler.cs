using Infrastructure.Api.Common;
using Infrastructure.Api.Messaging;
using Infrastructure.Api.Persistence;
using ProfileService.Command.Application.Abstractions;
using ProfileService.Command.Application.Commands;
using ProfileService.Command.Application.DTOs;

namespace ProfileService.Command.Application.Handlers;

public class UpdateProfilePictureHandler : ICommandHandler<UpdateProfilePictureCommand, ProfileResponse>
{
    private readonly IProfileRepository _profileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfilePictureHandler(IProfileRepository profileRepository, IUnitOfWork unitOfWork)
    {
        _profileRepository = profileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProfileResponse> HandleAsync(UpdateProfilePictureCommand command, CancellationToken cancellationToken = default)
    {
        var profile = command.ProfileId.HasValue
            ? await _profileRepository.GetByIdAsync(command.ProfileId.Value, cancellationToken)
            : command.AccountId.HasValue
                ? await _profileRepository.GetByAccountIdAsync(command.AccountId.Value, cancellationToken)
                : null;

        if (profile is null)
        {
            throw ApiException.NotFound("Profile not found.");
        }

        profile.UpdateProfilePicture(command.ProfilePictureUrl);
        await _profileRepository.UpdateAsync(profile, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return profile.ToResponse();
    }
}
