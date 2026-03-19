namespace Infrastructure.Api.Storage;

public interface IExternalFileStorageClient
{
    Task<ExternalFileUploadResult> UploadImageAsync(
        Stream content,
        string fileName,
        string contentType,
        string category,
        CancellationToken cancellationToken = default);
}
