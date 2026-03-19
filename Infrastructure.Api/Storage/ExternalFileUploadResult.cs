namespace Infrastructure.Api.Storage;

public sealed record ExternalFileUploadResult(
    string FileName,
    string ContentType,
    long Size,
    string StorageKey,
    string Url,
    DateTime UploadedAt);
