using System.Text;

namespace VoidNone.Storage.Internal;

public static partial class PathExtensions
{
    extension(Path)
    {
        internal static void ShouldBeRelative(string path)
        {
            if (Path.IsPathRooted(path))
            {
                throw new InvalidDataException($"Path '{path}' must be relative");
            }
        }

        internal static bool IsRelativeOrAbsolute(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;
            return IsRelative(path) || path.StartsWith('/');
        }

        internal static bool IsRelative(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;
            return path.StartsWith("./") || path.StartsWith("../");
        }

        public static string Normalization(string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            var pathBuilder = new StringBuilder();

            char? previous = null;
            for (int i = 0; i < path.Length; i++)
            {
                var @char = path[i];
                if (@char == '\\')
                {
                    @char = '/';
                }
                if (@char == '/' && previous == '/')
                {
                    continue;
                }
                pathBuilder.Append(@char);
                previous = @char;
            }

            return pathBuilder.ToString();
        }

#if NET6_0
        internal static bool Exists(string path)
        {
            return File.Exists(path) || Directory.Exists(path);
        }
#endif
    }

}