namespace VoidNone.Vone.Data.Storage.Files;

public class SearchEntryOptions
{
    public string? Pattern { get; set; }
    public bool? AllDirectories { get; set; }

    internal static SearchOption GetSearchOption(SearchEntryOptions? options)
    {
        if (options == null || options.AllDirectories == null) return SearchOption.TopDirectoryOnly;
        return options.AllDirectories.Value ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
    }
}