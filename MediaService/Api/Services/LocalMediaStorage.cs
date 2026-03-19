using Infrastructure.Api.Common;
using Infrastructure.Api.Storage;
using Microsoft.Extensions.Options;

namespace MediaService.Api.Services;

public class LocalMediaStorage : ILocalMediaStorage
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif"
    };

    private readonly LocalMediaStorageOptions _options;
    private readonly string _rootPath;

    public LocalMediaStorage(IWebHostEnvironment environment, IOptions<LocalMediaStorageOptions> options)
    {
        _options = options.Value;
        _rootPath = Path.IsPathRooted(_options.RootPath)
            ? _options.RootPath
            : Path.Combine(environment.ContentRootPath, _options.RootPath);

        Directory.CreateDirectory(_rootPath);
    }

    public string RootPath => _rootPath;

    public async Task<ExternalFileUploadResult> SaveImageAsync(
        string category,
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        if (!AllowedContentTypes.Contains(file.ContentType))
        {
            throw ApiException.BadRequest("Only JPEG, PNG, WEBP, and GIF images are supported.");
        }

        var maxBytes = _options.MaxFileSizeMb * 1024L * 1024L;
        if (file.Length > maxBytes)
        {
            throw ApiException.BadRequest($"Image exceeds the {_options.MaxFileSizeMb} MB limit.");
        }

        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = file.ContentType switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/webp" => ".webp",
                "image/gif" => ".gif",
                _ => ".bin"
            };
        }

        var normalizedCategory = string.IsNullOrWhiteSpace(category) ? "misc" : category.Trim().ToLowerInvariant();
        var relativeDirectory = Path.Combine(normalizedCategory, DateTime.UtcNow.ToString("yyyy"), DateTime.UtcNow.ToString("MM"));
        var absoluteDirectory = Path.Combine(_rootPath, relativeDirectory);
        Directory.CreateDirectory(absoluteDirectory);

        var generatedFileName = $"{Guid.NewGuid():N}{extension}";
        var absolutePath = Path.Combine(absoluteDirectory, generatedFileName);

        await using var stream = file.OpenReadStream();
        await using var target = File.Create(absolutePath);
        await stream.CopyToAsync(target, cancellationToken);

        var storageKey = string.Join('/', relativeDirectory.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).Append(generatedFileName));
        var url = $"{_options.PublicBaseUrl.TrimEnd('/')}/media/{storageKey}";

        return new ExternalFileUploadResult(
            file.FileName,
            file.ContentType,
            file.Length,
            storageKey,
            url,
            DateTime.UtcNow);
    }
}
