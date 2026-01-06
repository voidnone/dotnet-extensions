# VoidNone.Logging.Core

## Install

[![Nuget](https://img.shields.io/nuget/v/VoidNone.Logging.Core?label=nuget&style=for-the-badge)](https://www.nuget.org/packages/VoidNone.Logging.Core/)


## Usage

```
public class CustomLogWriter : ILogWriter
{
    public void WriteLog(Log log)
    {
        Console.WriteLine($"{log.Level} {log.Message} {log.Name} {log.Exception} {log.EventId}");
    }
}

//ILoggingBuilder
logging.AddImplementation<CustomLogWriter>();
```