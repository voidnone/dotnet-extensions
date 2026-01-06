using VoidNone.Logging.Core;

namespace Logging.CoreSample;

public class CustomLogWriter : ILogWriter
{
    public void WriteLog(Log log)
    {
        Console.WriteLine($"{log.Level} {log.Message} {log.Name} {log.Exception} {log.EventId}");
    }
}

