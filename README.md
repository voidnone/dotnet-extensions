# VoidNone.Logging

## Install

[![Nuget](https://img.shields.io/nuget/v/VoidNone.Logging.Core?label=nuget&style=for-the-badge)](https://www.nuget.org/packages/VoidNone.Logging.Core/)

[![Nuget](https://img.shields.io/nuget/v/VoidNone.Logging.File?label=nuget&style=for-the-badge)](https://www.nuget.org/packages/VoidNone.Logging.File/)



## VoidNone.Logging.Core

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

## VoidNone.Logging.File

```
//ILoggingBuilder
logging.AddFile();
```
Log will out in {application folder}/logs/20211020.txt
```
[Error] [LoggingFileSample.Worker] [0] [2021/10/20 01:05:35 +00:00]
Worker running at: 10/20/2021 21:05:35 +08:00
System.Exception: error

[Error] [LoggingFileSample.Worker] [0] [2021/10/20 01:05:36 +00:00]
Worker running at: 10/20/2021 21:05:36 +08:00
System.Exception: error

[Error] [LoggingFileSample.Worker] [0] [2021/10/20 01:05:37 +00:00]
Worker running at: 10/20/2021 21:05:37 +08:00
System.Exception: error
```