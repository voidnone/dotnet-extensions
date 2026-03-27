# VoidNone.Logging

Simple logging providers for `Microsoft.Extensions.Logging`.

This repository currently contains two packages:

- `VoidNone.Logging.Core`: plug in your own `ILogWriter`
- `VoidNone.Logging.File`: write logs to files by log level and date

## Install

[![Nuget](https://img.shields.io/nuget/v/VoidNone.Logging.Core?label=nuget&style=for-the-badge)](https://www.nuget.org/packages/VoidNone.Logging.Core/)

[![Nuget](https://img.shields.io/nuget/v/VoidNone.Logging.File?label=nuget&style=for-the-badge)](https://www.nuget.org/packages/VoidNone.Logging.File/)

```bash
dotnet add package VoidNone.Logging.Core
dotnet add package VoidNone.Logging.File
```

## VoidNone.Logging.Core

Use `VoidNone.Logging.Core` when you want to keep the standard logging API but send log entries to your own destination.

### Custom writer

```csharp
using VoidNone.Logging.Core;

public class CustomLogWriter : ILogWriter
{
    public void WriteLog(Log log)
    {
        Console.WriteLine($"{log.Level} {log.Message} {log.Name} {log.Exception} {log.EventId}");
    }
}
```

### Registration

```csharp
builder.Logging.ClearProviders();
builder.Logging.AddImplementation<CustomLogWriter>();
```

### Filtering

`VoidNone.Logging.Core` works with the standard `Logging` configuration model:

```json
{
  "Logging": {
    "Implementation": {
      "LogLevel": {
        "Default": "Error"
      }
    }
  }
}
```

## VoidNone.Logging.File

Use `VoidNone.Logging.File` when you want logs written to disk under the application directory.

### Registration

```csharp
builder.Logging.ClearProviders();
builder.Logging.AddFile();
```

You can also customize the file logger:

```csharp
builder.Logging.AddFile(options =>
{
    options.Path = "custom_log_folder";
    options.DateFormat = "yyyyMMdd";
    options.RetentionDays = 90;
});
```

### Options

| Option | Default | Description |
| --- | --- | --- |
| `Path` | `logs` | Base folder under `AppContext.BaseDirectory` |
| `DateFormat` | `yyyyMMdd` | Date pattern used in the file name |
| `RetentionDays` | `90` | Deletes files older than the configured number of days |

### Output path

Logs are written to:

```text
{application folder}/{Path}/{LogLevel}/{DateFormat}.log
```

With the default options, an error log written on `2021-10-20` will be stored at:

```text
{application folder}/logs/Error/20211020.log
```

### Example output

```text
[Error] [LoggingFileSample.Worker] [0] [2021/10/20 01:05:35 +00:00]
Worker running at: 10/20/2021 21:05:35 +08:00
System.Exception: error

[Error] [LoggingFileSample.Worker] [0] [2021/10/20 01:05:36 +00:00]
Worker running at: 10/20/2021 21:05:36 +08:00
System.Exception: error
```

### Filtering

```json
{
  "Logging": {
    "File": {
      "LogLevel": {
        "Default": "Information"
      }
    }
  }
}
```

## Samples

Sample applications are available under `samples/`:

- `samples/Logging.CoreSample`
- `samples/Logging.FileSample`
