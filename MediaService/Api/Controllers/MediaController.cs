using Infrastructure.Api.Common;
using MediaService.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MediaService.Api.Controllers;

[ApiController]
[Route("api/media/images")]
public class MediaController : ControllerBase
{
    private readonly ILocalMediaStorage _storage;
    private readonly IConfiguration _configuration;

    public MediaController(ILocalMediaStorage storage, IConfiguration configuration)
    {
        _storage = storage;
        _configuration = configuration;
    }

    [HttpPost("{category}")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> UploadImage(string category, IFormFile? file, CancellationToken cancellationToken)
    {
        var configuredApiKey = _configuration["Storage:InternalApiKey"];
        if (!string.IsNullOrWhiteSpace(configuredApiKey))
        {
            var providedApiKey = Request.Headers["X-Internal-Api-Key"].ToString();
            if (!string.Equals(providedApiKey, configuredApiKey, StringComparison.Ordinal))
            {
                throw ApiException.Unauthorized("Invalid media API key.");
            }
        }

        if (file is null || file.Length == 0)
        {
            throw ApiException.BadRequest("A non-empty file is required.");
        }

        var result = await _storage.SaveImageAsync(category, file, cancellationToken);
        return Ok(result);
    }
}
