# Microsoft DependencyInjection extensions

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
