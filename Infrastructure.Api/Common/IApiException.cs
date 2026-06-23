namespace Infrastructure.Api.Common;

public interface IApiException
{
    int StatusCode { get; }
    string Message { get; }
    object? Details { get; }
}
