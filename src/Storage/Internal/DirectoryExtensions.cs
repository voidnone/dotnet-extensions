namespace VoidNone.Storage.Internal;

public static class DirectoryExtensions
{
    extension(Directory)
    {
        public static void EnsureDirectoryExist(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;

            if (File.Exists(path))
            {
                throw new InvalidOperationException($"Path '{path}' file exist");
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void EnsureFileDirectoryExist(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            var directory = Path.GetDirectoryName(path);
            if (directory != null)
            {
                Directory.EnsureDirectoryExist(directory);
            }
        }
    }
}