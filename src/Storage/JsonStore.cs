using System.Text.Json;
using VoidNone.Storage.Internal;

namespace VoidNone.Storage;

public class JsonStore
{
    private readonly string root;

    public JsonStore(string root)
    {
        this.root = root;
        Directory.EnsureDirectoryExist(root);
    }

    public T? Get<T>() => Get<T>(typeof(T).Name);

    public T? Get<T>(string name)
    {
        var path = Path.Combine(root, $"{name}.json");
        if (!File.Exists(path)) return default;
        var result = JsonSerializer.DeserializeFile<T>(path, JsonSerializerOptions.App);
        return result;
    }

    public async Task<T?> GetAsync<T>(CancellationToken token = default) => await GetAsync<T>(typeof(T).Name, token);

    public async Task<T?> GetAsync<T>(string name, CancellationToken token = default)
    {
        var path = Path.Combine(root, $"{name}.json");
        if (!File.Exists(path)) return default;
        var result = await JsonSerializer.DeserializeFileAsync<T>(path, JsonSerializerOptions.Readable, token);
        return result;
    }

    public T GetRequired<T>() => GetRequired<T>(typeof(T).Name);

    public T GetRequired<T>(string name)
    {
        var result = Get<T>(name) ?? throw new DataCanNotBeNullException();
        return result;
    }

    public async Task<T> GetRequiredAsync<T>(CancellationToken token = default)
    {
        return await GetRequiredAsync<T>(typeof(T).Name, token);
    }

    public async Task<T> GetRequiredAsync<T>(string name, CancellationToken token = default)
    {
        var result = await GetAsync<T>(name, token) ?? throw new DataCanNotBeNullException();
        return result;
    }

    public bool Exist<T>()
    {
        return Exist(typeof(T).Name);
    }

    public bool Exist(string name)
    {
        var path = Path.Combine(root, $"{name}.json");
        return File.Exists(path);
    }

    public async Task SaveAsync<T>(object value, CancellationToken token = default)
    {
        var path = Path.Combine(root, $"{typeof(T).Name}.json");
        await JsonSerializer.SerializeFileAsync(path, value, JsonSerializerOptions.Readable, token);
    }

    public async Task SaveAsync(string name, object value, CancellationToken token = default)
    {
        var path = Path.Combine(root, $"{name}.json");
        await JsonSerializer.SerializeFileAsync(path, value, JsonSerializerOptions.Readable, token);
    }

    public void Delete(string name)
    {
        var path = Path.Combine(root, $"{name}.json");
        if (File.Exists(path)) File.Delete(path);
    }
}

public class JsonStore<T>
{
    private readonly string rootPath;

    public JsonStore(string root, string? directory = default)
    {
        rootPath = Path.Combine(root, directory ?? typeof(T).Name);
        Directory.EnsureDirectoryExist(rootPath);
    }

    public async Task<IDictionary<string, T>> ListAsync(CancellationToken token = default)
    {
        var files = Directory.GetFiles(rootPath, "*.json");
        var result = new Dictionary<string, T>();

        foreach (var item in files)
        {
            var value = await JsonSerializer.DeserializeFileAsync<T>(item, JsonSerializerOptions.Readable, token);
            var key = Path.GetFileNameWithoutExtension(item);
            if (value != null) result.Add(key, value);
        }

        return result;
    }

    public async Task SaveAsync(string name, T value, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(value);
        var path = Path.Combine(rootPath, $"{name}.json");
        await JsonSerializer.SerializeFileAsync(path, value, JsonSerializerOptions.Readable, token);
    }

    public async Task<T?> GetAsync(string name, CancellationToken token = default)
    {
        var path = Path.Combine(rootPath, $"{name}.json");
        var result = await JsonSerializer.DeserializeFileAsync<T>(path, JsonSerializerOptions.Readable, token);
        return result;
    }

    public async Task<T> GetRequiredAsync(string name, CancellationToken token = default)
    {
        var path = Path.Combine(rootPath, $"{name}.json");
        var result = await JsonSerializer.DeserializeFileAsync<T>(path, JsonSerializerOptions.Readable, token) ?? throw new DataCanNotBeNullException();
        return result;
    }

    public bool Exist(string name)
    {
        var path = Path.Combine(rootPath, $"{name}.json");
        return File.Exists(path);
    }

    public void ThrowIfNotExist(string name)
    {
        if (Exist(name)) return;
        throw new DataCanNotBeNullException();
    }

    public void Delete(string name)
    {
        var path = Path.Combine(rootPath, $"{name}.json");
        if (File.Exists(path)) File.Delete(path);
    }
}