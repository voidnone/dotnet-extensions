# VoidNone.DependencyInjection

## Install

[![Nuget](https://img.shields.io/nuget/v/VoidNone.DependencyInjection?label=nuget&style=for-the-badge)](https://www.nuget.org/packages/VoidNone.DependencyInjection/)

## Usage

```
[Transient(typeof(IService))]
class Service : IService { }
interface IService { }

//ServiceCollection
services.AddFromAssemblies(typeof(Service).Assembly)

//IServiceProvider
var service = _services.GetService<IService>();
```
