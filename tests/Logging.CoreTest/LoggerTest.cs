using Microsoft.Extensions.Logging;
using VoidNone.Logging.Core;

namespace VoidNone.Logging.CoreTest;

[TestClass]
public class LoggerTest
{
    private class CaptureLogWriter : ILogWriter
    {
        public List<Log> Logs { get; } = [];

        public void WriteLog(Log log)
        {
            Logs.Add(log);
        }
    }

    [TestMethod]
    public void Log_ShouldWriteWhenMessageIsEmptyButExceptionExists()
    {
        var logWriter = new CaptureLogWriter();
        var logger = new Logger("LoggerTest", null, logWriter);
        var exception = new InvalidOperationException("boom");

        logger.Log(
            LogLevel.Error,
            new EventId(1),
            string.Empty,
            exception,
            static (_, _) => string.Empty
        );

        Assert.HasCount(1, logWriter.Logs);
        Assert.AreSame(exception, logWriter.Logs[0].Exception);
        Assert.AreEqual(string.Empty, logWriter.Logs[0].Message);
    }

    [TestMethod]
    public void Log_ShouldSkipWhenMessageAndExceptionAreBothEmpty()
    {
        var logWriter = new CaptureLogWriter();
        var logger = new Logger("LoggerTest", null, logWriter);

        logger.Log(
            LogLevel.Information,
            new EventId(2),
            string.Empty,
            null,
            static (_, _) => string.Empty
        );

        Assert.HasCount(0, logWriter.Logs);
    }

    [TestMethod]
    public void Log_ShouldRespectFilter()
    {
        var logWriter = new CaptureLogWriter();
        var logger = new Logger("LoggerTest", (_, level) => level >= LogLevel.Error, logWriter);

        logger.Log(
            LogLevel.Information,
            new EventId(3),
            "filtered",
            null,
            static (state, _) => state
        );

        Assert.HasCount(0, logWriter.Logs);
    }
}
