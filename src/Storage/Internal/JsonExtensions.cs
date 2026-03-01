using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VoidNone.Storage.Internal;

public static class JsonExtensions
{
    private static readonly JsonSerializerOptions appOptions = new(JsonSerializerDefaults.Web);
    private static readonly JsonSerializerOptions readableOptions = new(JsonSerializerDefaults.Web);

    static JsonExtensions()
    {
        ConfigureAppOptions(appOptions);
        ConfigureReadableOptions(readableOptions);
    }

    extension(JsonSerializer)
    {
        public static void SerializeFile(string path, object value, JsonSerializerOptions options)
        {
            using var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            stream.SetLength(0);
            JsonSerializer.Serialize(stream, value, options);
        }

        public static async Task SerializeFileAsync(string path, object value, JsonSerializerOptions options, CancellationToken token = default)
        {
            using var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            stream.SetLength(0);
            await JsonSerializer.SerializeAsync(stream, value, options, token);
        }

        public static T? DeserializeFile<T>(string path, JsonSerializerOptions options)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return JsonSerializer.Deserialize<T>(stream, options);
        }

        public static async ValueTask<T?> DeserializeFileAsync<T>(string path, JsonSerializerOptions options, CancellationToken token = default)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return await JsonSerializer.DeserializeAsync<T>(stream, options, token);
        }
    }

    extension(JsonSerializerOptions)
    {
        public static JsonSerializerOptions Readable => readableOptions;
        public static JsonSerializerOptions App => appOptions;

        public static void ConfigureAppOptions(JsonSerializerOptions options)
        {
            options.PropertyNameCaseInsensitive = true;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.NumberHandling = JsonNumberHandling.AllowReadingFromString;
            options.Converters.Add(new JsonStringEnumConverter());
        }

        public static void ConfigureReadableOptions(JsonSerializerOptions options)
        {
            ConfigureAppOptions(options);
            options.AllowTrailingCommas = true;
            options.ReadCommentHandling = JsonCommentHandling.Skip;
            options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            options.WriteIndented = true;
        }
    }
}