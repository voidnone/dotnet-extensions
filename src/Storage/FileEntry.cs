namespace VoidNone.Storage;

public class FileEntry : Entry
{
    private readonly FileInfo fileInfo;
    private readonly Lazy<string> contentType;

    public FileEntry(FileInfo fileInfo, string root) : base(fileInfo, root)
    {
        this.fileInfo = fileInfo;

        contentType = new Lazy<string>(() =>
        {
            if (MimeMapping.TryGetContentType(RelationPath, out var contentType))
            {
                return contentType;
            }
            return "application/octet-stream";
        }, true);
    }

    public override long Size => fileInfo.Length;

    public string ContentType => contentType.Value;

    public override bool IsDirectory => false;

    public override bool IsFile => true;
}