using System;
using System.Collections.Generic;
using System.Text;

namespace Huanent.Logging.Sqlite;

public class SqliteLoggerOptions
{
    public string Path { get; set; } = "logs";
    public string DateFormat { get; set; } = "yyyyMM";
}