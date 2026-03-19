using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;

namespace Infrastructure.Api.Mapping;

public static class JsonElementMapper
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly NullabilityInfoContext NullabilityContext = new();
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> RequiredStringPropertiesCache = new();

    public static TDestination Map<TDestination>(JsonElement payload, Action<JsonElement, TDestination>? customize = null)
        where TDestination : class, new()
    {
        var model = payload.Deserialize<TDestination>(SerializerOptions) ?? new TDestination();

        NormalizeRequiredStrings(model);
        customize?.Invoke(payload, model);

        return model;
    }

    public static Guid GetRequiredGuid(this JsonElement payload, string propertyName) =>
        payload.GetProperty(propertyName).GetGuid();

    public static Guid? GetOptionalGuid(this JsonElement payload, string propertyName) =>
        payload.TryGetProperty(propertyName, out var property) && property.ValueKind != JsonValueKind.Null
            ? property.GetGuid()
            : null;

    public static DateTime? GetOptionalDateTime(this JsonElement payload, string propertyName) =>
        payload.TryGetProperty(propertyName, out var property) && property.ValueKind != JsonValueKind.Null
            ? property.GetDateTime()
            : null;

    public static string? GetOptionalString(this JsonElement payload, string propertyName) =>
        payload.TryGetProperty(propertyName, out var property) && property.ValueKind != JsonValueKind.Null
            ? property.GetString()
            : null;

    private static void NormalizeRequiredStrings<TDestination>(TDestination model)
        where TDestination : class
    {
        var properties = RequiredStringPropertiesCache.GetOrAdd(
            typeof(TDestination),
            static type => type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(property =>
                    property.PropertyType == typeof(string) &&
                    property.CanRead &&
                    property.CanWrite &&
                    NullabilityContext.Create(property).WriteState == NullabilityState.NotNull)
                .ToArray());

        foreach (var property in properties)
        {
            if (property.GetValue(model) is null)
            {
                property.SetValue(model, string.Empty);
            }
        }
    }
}
