# VoidNone.DependencyInjection

VoidNone.DependencyInjection is a lightweight .NET dependency injection extension library that supports automatic service registration via attributes. It simplifies the service registration process, improves development efficiency, and is suitable for various .NET application scenarios.

## Install

[![Nuget](https://img.shields.io/nuget/v/VoidNone.DependencyInjection?label=nuget&style=for-the-badge)](https://www.nuget.org/packages/VoidNone.DependencyInjection/)

## Usage

### Service Definition

Define a class as a service

```csharp
[Transient]
class TransientService { }

[Singleton]
class SingletonService { }

[Scoped]
class ScopedService { }
```

Using interfaces

```csharp
interface IBaseService { }

interface IService : IBaseService { }

[Singleton<IService>]
class Service1 : IService { }

// or

[Singleton<IService, IBaseService>]
class Service2 : IService { }

// or

[Singleton(typeof(IService), typeof(IBaseService))]
class Service3 : IService { }
```

Registering keyed services

```csharp
[Singleton<IService>("keyName")]
class Service1 : IService { }

// or

[Singleton("keyName", typeof(IService))]
class Service2 : IService { }
```

### Registering Services

```csharp
// ServiceCollection
services.AddFromAssemblies(typeof(Service1).Assembly)
```

### Extended Service Retrieval Methods

Retrieve all service types, including regular and keyed services

```csharp
_services.GetAllServiceTypes();
```

Retrieve all service instances, including regular and keyed services

```csharp
_services.GetAllServiceInstances();
```

Retrieve the ServiceProvider for the current HttpContext scope

```csharp
_services.GetScopedServiceProvider();
```
