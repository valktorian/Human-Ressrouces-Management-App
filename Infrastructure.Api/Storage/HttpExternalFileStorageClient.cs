using System.Net.Http.Headers;
using System.Net.Http.Json;
using Infrastructure.Api.Common;
using Microsoft.Extensions.Options;

namespace Infrastructure.Api.Storage;

public class HttpExternalFileStorageClient : IExternalFileStorageClient
{
    private readonly HttpClient _httpClient;
    private readonly ExternalFileStorageOptions _options;

    public HttpExternalFileStorageClient(HttpClient httpClient, IOptions<ExternalFileStorageOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<ExternalFileUploadResult> UploadImageAsync(
        Stream content,
        string fileName,
        string contentType,
        string category,
        CancellationToken cancellationToken = default)
    {
        using var multipart = new MultipartFormDataContent();
        using var fileContent = new StreamContent(content);

        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        multipart.Add(fileContent, "file", fileName);

        using var request = new HttpRequestMessage(HttpMethod.Post, $"api/media/images/{Uri.EscapeDataString(category)}")
        {
            Content = multipart
        };

        if (!string.IsNullOrWhiteSpace(_options.InternalApiKey))
        {
            request.Headers.Add("X-Internal-Api-Key", _options.InternalApiKey);
        }

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new ApiException(
                string.IsNullOrWhiteSpace(body) ? "File upload failed." : body,
                (int)response.StatusCode);
        }

        var result = await response.Content.ReadFromJsonAsync<ExternalFileUploadResult>(cancellationToken: cancellationToken);
        return result ?? throw ApiException.Internal("File upload response was empty.");
    }
}
