using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VoidNone.Logging.Core;
using VoidNone.Logging.File;

namespace VoidNone.Logging.CoreTest;

[TestClass]
public class FileLogWriterTest
{
    [TestMethod]
    public void WriteLog_ShouldSucceedWhenLogDirectoryDoesNotExist()
    {
        var relativePath = Path.Combine("test-logs", Guid.NewGuid().ToString("N"));
        var rootDirectory = Path.Combine(AppContext.BaseDirectory, relativePath);

        if (Directory.Exists(rootDirectory))
        {
            Directory.Delete(rootDirectory, true);
        }

        var fileLogWriter = new FileLogWriter(Options.Create(new FileLoggerOptions
        {
            Path = relativePath,
        }));

        try
        {
            var creationTime = new DateTimeOffset(2024, 11, 12, 13, 14, 15, TimeSpan.Zero);
            fileLogWriter.WriteLog(new Log(LogLevel.Error, "FileLogWriterTest", new EventId(1), "first log", null)
            {
                CreationTime = creationTime,
            });
            fileLogWriter.Dispose();

            var logFilePath = Path.Combine(rootDirectory, LogLevel.Error.ToString(), $"{creationTime:yyyyMMdd}.log");
            Assert.IsTrue(System.IO.File.Exists(logFilePath));

            var content = System.IO.File.ReadAllText(logFilePath);
            StringAssert.Contains(content, "[Error] [FileLogWriterTest] [1] [2024/11/12 13:14:15 +00:00]");
            StringAssert.Contains(content, "first log");
        }
        finally
        {
            if (Directory.Exists(rootDirectory))
            {
                Directory.Delete(rootDirectory, true);
            }
        }
    }

    [TestMethod]
    public void WriteLog_ShouldAppendExceptionDetails()
    {
        var relativePath = Path.Combine("test-logs", Guid.NewGuid().ToString("N"));
        var rootDirectory = Path.Combine(AppContext.BaseDirectory, relativePath);
        var exception = new InvalidOperationException("file logger failure");

        var fileLogWriter = new FileLogWriter(Options.Create(new FileLoggerOptions
        {
            Path = relativePath,
        }));

        try
        {
            var creationTime = new DateTimeOffset(2024, 11, 12, 13, 14, 16, TimeSpan.Zero);
            fileLogWriter.WriteLog(new Log(LogLevel.Error, "FileLogWriterTest", new EventId(2), "second log", exception)
            {
                CreationTime = creationTime,
            });
            fileLogWriter.Dispose();

            var logFilePath = Path.Combine(rootDirectory, LogLevel.Error.ToString(), $"{creationTime:yyyyMMdd}.log");
            var content = System.IO.File.ReadAllText(logFilePath);

            StringAssert.Contains(content, "second log");
            StringAssert.Contains(content, exception.ToString());
        }
        finally
        {
            if (Directory.Exists(rootDirectory))
            {
                Directory.Delete(rootDirectory, true);
            }
        }
    }
}
