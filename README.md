# VoidNone.Extensions

A lightweight collection of .NET extension libraries providing dependency injection helpers and logging components. These packages are intended to be modular, testable, and suitable for small to medium-sized applications.

## Packages

| Name | Description |
| --- | --- |
| VoidNone.DependencyInjection | Attribute-driven service registration and resolution helpers. Provides attributes to declare service lifetimes (Singleton, Scoped, Transient) and automates registration via assembly scanning. Ideal for projects that prefer convention and attribute-based registration. |
| VoidNone.Logging.Core | Core logging abstractions and interfaces, including `ILogger`/`ILogWriter`-style contracts and factories. Designed to allow swapping implementations and to ease writing test doubles. |
| VoidNone.Logging.File | File-based logging implementation and factory extensions. Supports rolling by date or size, formatting templates, and simple configuration options for common file logging scenarios. |