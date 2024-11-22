# Table of Contents

1. [DI Container](#di-container)
2. [Initializator](#initializator)
3. [Event Bus](#event-bus)


# [DI Container](./DiContainer)

The `Container` is a simple dependency injection (DI) system. It allows for the resolution of instances by type.

## Usage

### 1. `Bind<T>(T instance)`

Binds an instance of type `T` to the container. If an instance of type `T` is already registered, an exception is thrown.

#### Example Usage:

```csharp
Container.Bind(new FlyItemsService());
```

- Binds an instance of `FlyItemsService` and all its base types (e.g., `IFlyItemsService`).

---

### 2. `Resolve<T>()`, `Resolve(Type instance)`

Resolves and returns an instance of the requested type `T` from the container. If the requested service is not registered, an exception is thrown.
Both base and final instance types are suitable for resolution.

#### Example Usage:

```csharp
IUIService uiServiceBase = Container.Resolve<IUIService>(); // Resolve via base type 
UIService uiServiceImpl = Container.Resolve<UIService>(); // Resolve via implementation type
```
---

### 3. `Inject<T>(T instance)`

Injects dependencies into the provided instance. The injectable class should have a method marked with `[Inject]` to receive dependencies.

#### Example Usage:

```csharp
public class FlyItemsService : IFlyItemsService
{
    [Inject]
    private void Inject(IUIService uiService)
    {
        UiService = uiService;
    }
}

var service = Container.Bind(new FlyItemsService());
Container.Inject(service);
```
- Injects `IUIService` into an instance of `FlyItemsService`.

---

### 4. `UnBind<T>()`

Unbinds and removes an instance of type `T` from the container. If the instance implements `IDisposable`, it will be disposed of before being removed. Throws an `InvalidOperationException` if no instance of type `T` is found.

#### Example Usage:

```csharp
Container.UnBind<IFlyItemsService>();
```

- Removes the `IFlyItemsService` instance from the container and disposes of it if necessary.

---

# [Initializator](./Initializator)

This class perform instances initialization in proper order. 
1. It recursevly collect dependencies from methods marked with `[Inject]` attribute
2. Build [dependency graph](./Initializator/DependencyGraph.cs)
3. Calculate proper order
4. Call `IInitializable.Initialize()` methods in that order

#### Example Usage:

```csharp
  // collect all initializable instances
  var initializableServices = Container.AllInstances<IInitializable>();

  // Initialize then in proper order
  await new Initializator(initializableServices).Do();
```
---

# Event Bus

[Events](./Events/Event.cs)

An event bus allows publishing and receiving events across subsystems

#### Example Usage:

```csharp
  // publish
  Event<LocalizationChangedEvent>.Publish();

  // subscribe 
  Event<LocalizationChangedEvent>.Subscribe(OnLocalizationChanged);

  // event waiter
  await Event<LocalizationChangedEvent>.Wait();
```
---
