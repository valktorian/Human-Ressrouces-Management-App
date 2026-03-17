using Infrastructure.Api.Common;
using Infrastructure.Api.Messaging;
using Infrastructure.Api.Persistence;
using ProfileService.Command.Application.Abstractions;
using ProfileService.Command.Application.Commands;

namespace ProfileService.Command.Application.Handlers;

public class DeleteProfileHandler : ICommandHandler<DeleteProfileCommand, bool>
{
    private readonly IProfileRepository _profileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProfileHandler(IProfileRepository profileRepository, IUnitOfWork unitOfWork)
    {
        _profileRepository = profileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> HandleAsync(DeleteProfileCommand command, CancellationToken cancellationToken = default)
    {
        var profile = await _profileRepository.GetByIdAsync(command.ProfileId, cancellationToken)
            ?? throw ApiException.NotFound("Profile not found.");

        await _profileRepository.DeleteAsync(profile, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
