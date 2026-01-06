namespace VoidNone.Logging.File;

public class FileLoggerOptions
{
    public string Path { get; set; } = "logs";
    public string DateFormat { get; set; } = "yyyyMMdd";
}