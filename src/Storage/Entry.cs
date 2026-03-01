using System.Text.Json.Serialization;

namespace VoidNone.Storage;

#if NET8_0_OR_GREATER
[JsonDerivedType(typeof(FileEntry))]
[JsonDerivedType(typeof(DirectoryEntry))]
#endif
public abstract class Entry
{
    private readonly FileSystemInfo fileSystemInfo;
    private readonly Lazy<string> relationPath;

    public Entry(FileSystemInfo fileSystemInfo, string root)
    {
        this.fileSystemInfo = fileSystemInfo;
        relationPath = new Lazy<string>(() => Path.GetRelativePath(root, fileSystemInfo.FullName), true);
    }

    public string Name => fileSystemInfo.Name;

    public DateTime CreationTime => fileSystemInfo.CreationTimeUtc;

    public DateTime LastWriteTime => fileSystemInfo.LastWriteTimeUtc;

    public string Extension => fileSystemInfo.Extension;

    [JsonIgnore]
    public string FullPath => fileSystemInfo.FullName;

    public abstract bool IsDirectory { get; }

    public abstract bool IsFile { get; }

    public virtual long Size { get; }

    public string RelationPath => relationPath.Value;
}