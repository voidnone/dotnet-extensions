using Huanent.Logging.Core;

namespace Logging.CoreSample
{
    public class CustomLogWriter : ILogWriter
    {
        public void WriteLog(LogLevel level, string message, string name, Exception exception, EventId eventId)
        {
            Console.WriteLine($"{level} {message} {name} {exception} {eventId}");
        }
    }
}
