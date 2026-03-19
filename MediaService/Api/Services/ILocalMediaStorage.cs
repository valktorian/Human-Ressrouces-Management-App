using Infrastructure.Api.Storage;

namespace MediaService.Api.Services;

public interface ILocalMediaStorage
{
    string RootPath { get; }

    Task<ExternalFileUploadResult> SaveImageAsync(
        string category,
        IFormFile file,
        CancellationToken cancellationToken = default);
}
