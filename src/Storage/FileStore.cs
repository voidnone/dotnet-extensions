using VoidNone.Storage.Internal;
using VoidNone.Vone.Data.Storage.Files;

namespace VoidNone.Storage;

public class FileStore
{
    private readonly string root;
    public string Root => root;

    public FileStore(string root)
    {
        Directory.EnsureDirectoryExist(root);
        this.root = root;
    }

    public Entry GetEntry(string path)
    {
        path = GetValidPath(path);
        if (!Path.Exists(path))
        {
            throw new PathNotFoundException();
        }

        var fileInfo = new FileInfo(path);
        if (fileInfo.Exists)
        {
            return new FileEntry(fileInfo, root);
        }

        var directoryInfo = new DirectoryInfo(path);
        return new DirectoryEntry(directoryInfo, root);
    }

    public IEnumerable<Entry> GetEntries(string path, SearchEntryOptions? options = null)
    {
        path = GetValidPath(path);
        var option = SearchEntryOptions.GetSearchOption(options);
        var pattern = options?.Pattern ?? "*";

        foreach (var entry in Directory.EnumerateDirectories(path, pattern, option))
        {
            var directoryInfo = new DirectoryInfo(entry);
            yield return new DirectoryEntry(directoryInfo, root);

        }

        foreach (var entry in Directory.EnumerateFiles(path, pattern, option))
        {
            var fileInfo = new FileInfo(entry);
            yield return new FileEntry(fileInfo, root);
        }
    }
    public FileEntry GetFile(string path)
    {
        path = GetValidPath(path);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException();
        }

        var fileInfo = new FileInfo(path);
        return new FileEntry(fileInfo, root);
    }

    public IEnumerable<FileEntry> GetFiles(string path, SearchEntryOptions? options = null)
    {
        path = GetValidPath(path);
        var option = SearchEntryOptions.GetSearchOption(options);
        var files = Directory.EnumerateFiles(path, options?.Pattern ?? "*", option);

        foreach (var item in files)
        {
            var fileInfo = new FileInfo(item);
            yield return new FileEntry(fileInfo, root);
        }
    }

    public DirectoryEntry GetDirectory(string path)
    {
        path = GetValidPath(path);

        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException();
        }

        var directoryInfo = new DirectoryInfo(path);
        return new DirectoryEntry(directoryInfo, root);
    }

    public IEnumerable<DirectoryEntry> GetDirectories(string path, SearchEntryOptions? options = null)
    {
        path = GetValidPath(path);
        var option = SearchEntryOptions.GetSearchOption(options);
        var directories = Directory.EnumerateDirectories(path, options?.Pattern ?? "*", option);

        foreach (var item in directories)
        {
            var directoryInfo = new DirectoryInfo(item);
            yield return new DirectoryEntry(directoryInfo, root);
        }
    }

    public string CreateDirectory(string path)
    {
        path = GetValidPath(path);
        Directory.EnsureDirectoryExist(path);
        return path;
    }

    public async Task SaveFileAsync(string path, Stream stream)
    {
        path = GetValidPath(path);
        Directory.EnsureFileDirectoryExist(path);
        using var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        fs.SetLength(0);
        await stream.CopyToAsync(fs);
    }

    public async Task SaveFileAsync(string path, string content)
    {
        path = GetValidPath(path);
        Directory.EnsureFileDirectoryExist(path);
        await File.WriteAllTextAsync(path, content);
    }

    public FileStream GetFileWriteStream(string path)
    {
        path = GetValidPath(path);
        var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        stream.SetLength(0);
        return stream;
    }

    public FileStream GetFileReadStream(string path)
    {
        path = GetValidPath(path);
        return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public async Task<string> ReadTextAsync(string path)
    {
        path = Path.Combine(root, path);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException();
        }

        return await File.ReadAllTextAsync(path);
    }

    public void Delete(string[] paths)
    {
        foreach (var item in paths)
        {
            var path = GetValidPath(item);
            var isDirectory = Directory.Exists(path);
            if (isDirectory && Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            else if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    public void Move(string source, string target)
    {
        source = GetValidPath(source);
        target = GetValidPath(target);
        if (Directory.Exists(source))
        {
            if (Directory.Exists(target) && source == target)
            {
                throw new DirectoryExistException();
            }

            Directory.Move(source, target);
        }
        else if (File.Exists(source))
        {
            if (File.Exists(target) && source == target)
            {
                throw new FileExistException();
            }

            File.Move(source, target);
        }
        else
        {
            throw new PathNotFoundException();
        }
    }

    public bool Exists(string path)
    {
        path = GetValidPath(path);
        if (File.Exists(path)) return true;
        if (Directory.Exists(path)) return true;
        return false;
    }

    public string? GetSameFile(string path, string[]? extensions = null)
    {
        path = GetValidPath(path);
        if (File.Exists(path)) return path;
        if (extensions == null) return null;

        foreach (var item in extensions)
        {
            var pathWithExtension = path + item;
            if (File.Exists(pathWithExtension)) return pathWithExtension;
        }

        return null;
    }

    protected string GetValidPath(string path)
    {
        Path.ShouldBeRelative(path);
        path = Path.Combine(root, path);
        path = Path.Normalization(path);
        return path;
    }
}