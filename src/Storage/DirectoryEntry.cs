namespace VoidNone.Storage;

public class DirectoryEntry(DirectoryInfo directoryInfo, string root) : Entry(directoryInfo, root)
{
    public override bool IsDirectory => true;

    public override bool IsFile => false;
}