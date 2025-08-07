using Refit;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mav.Healthcheck.Infrastructure;

public static class JsonDefaults
{
    public static JsonSerializerOptions DefaultOptionsWithStringEnumConversion = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    public static JsonSerializerOptions DefaultOptionsWithCaseInsensitive = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static JsonSerializerOptions PropertyNamingPolicyAndWriteIndented = new()
    {
        PropertyNamingPolicy = null, // Pascal
        WriteIndented = true
    };

    public static readonly SystemTextJsonContentSerializer DefaultSystemTextJsonContentSerializer
        = new(DefaultOptionsWithStringEnumConversion);
}
