using Infrastructure.Api.Common;
using Infrastructure.Api.Messaging;
using Infrastructure.Api.Persistence;
using ProfileService.Command.Application.Abstractions;
using ProfileService.Command.Application.Commands;
using ProfileService.Command.Application.DTOs;

namespace ProfileService.Command.Application.Handlers;

public class UpdateProfileStatusHandler : ICommandHandler<UpdateProfileStatusCommand, ProfileResponse>
{
    private readonly IProfileRepository _profileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileStatusHandler(IProfileRepository profileRepository, IUnitOfWork unitOfWork)
    {
        _profileRepository = profileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProfileResponse> HandleAsync(UpdateProfileStatusCommand command, CancellationToken cancellationToken = default)
    {
        var profile = await _profileRepository.GetByIdAsync(command.ProfileId, cancellationToken)
            ?? throw ApiException.NotFound("Profile not found.");

        profile.UpdateStatus(command.EmploymentStatus);
        await _profileRepository.UpdateAsync(profile, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return profile.ToResponse();
    }
}
