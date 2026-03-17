namespace Infrastructure.Api.Common;

public class PaginationRequest
{
    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    public int PageNumber { get; init; } = DefaultPageNumber;
    public int PageSize { get; init; } = DefaultPageSize;

    public int NormalizedPageNumber => PageNumber < 1 ? DefaultPageNumber : PageNumber;

    public int NormalizedPageSize
    {
        get
        {
            if (PageSize < 1)
            {
                return DefaultPageSize;
            }

            return PageSize > MaxPageSize ? MaxPageSize : PageSize;
        }
    }

    public int Skip => (NormalizedPageNumber - 1) * NormalizedPageSize;
}
