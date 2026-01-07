# VoidNone Dotnet Extensions

A lightweight collection of .NET extension libraries.

## Packages

| Name | Package | Description |
| --- | --- | --- |
| [VoidNone.DependencyInjection](./src/DependencyInjection/README.md) | [![Nuget](https://img.shields.io/nuget/v/VoidNone.DependencyInjection?label=nuget&style=for-the-badge)](https://www.nuget.org/packages/VoidNone.DependencyInjection/) | Attribute-driven service registration and resolution helpers. Provides attributes to declare service lifetimes (Singleton, Scoped, Transient) and automates registration via assembly scanning. Ideal for projects that prefer convention and attribute-based registration. |
| [VoidNone.Logging.Core](./src/Logging.Core/README.md) | [![Nuget](https://img.shields.io/nuget/v/VoidNone.Logging.Core?label=nuget&style=for-the-badge)](https://www.nuget.org/packages/VoidNone.Logging.Core/) | Core logging abstractions and interfaces, including `ILogger`/`ILogWriter`-style contracts and factories.|
| [VoidNone.Logging.File](./src/Logging.File/README.md) | [![Nuget](https://img.shields.io/nuget/v/VoidNone.Logging.File?label=nuget&style=for-the-badge)](https://www.nuget.org/packages/VoidNone.Logging.File/) | File-based logging implementation and factory extensions. 