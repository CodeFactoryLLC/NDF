# Net Delivery Framework (NDF)

[![NuGet](https://img.shields.io/nuget/v/CodeFactory.NDF)](https://www.nuget.org/packages/CodeFactory.NDF)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

**CodeFactory.NDF** is a common delivery framework for .NET solutions, providing reusable patterns for managed exception handling, structured logging, and dependency injection loading across libraries and applications.

---

## Supported Frameworks

| Framework     | Version |
|---------------|---------|
| .NET Standard | 2.1     |
| .NET          | 10.0    |

---

## Features

- ✅ **Managed Exception Hierarchy** — A structured set of application-safe exceptions derived from a common base class, designed to surface safe messages to end users without leaking internal details.
- ✅ **Aggregated Exception Support** — Collect and raise multiple managed exceptions together as a single unit.
- ✅ **Structured Logging Extensions** — Extended `ILogger` methods that capture member names, line numbers, and entry/exit logging automatically via caller info attributes.
- ✅ **Dependency Injection Loader** — An abstract base class for library-level DI registration, supporting layered loading of registrations across library hierarchies.

---

## Installation

```
dotnet add package CodeFactory.NDF
```


Or via the NuGet Package Manager in Visual Studio by searching for **CodeFactory.NDF**.

---

## Managed Exceptions

The NDF managed exception system provides a structured, application-safe exception hierarchy. All exceptions derive from `ManagedException`, which itself extends `System.Exception`. The intent is to ensure that only safe, user-appropriate messages are surfaced through exceptions, while still allowing inner exceptions to carry full internal diagnostic detail.

### Design Principles

- **Application-safe messages** — Every exception type ships a default message via `StandardExceptionMessages` (a strongly-typed resource class). These messages are designed to be safe to display to end users.
- **Custom messages** — Every exception type supports construction with a custom message for more specific scenarios.
- **Inner exception chaining** — Every exception type accepts an optional `internalException` parameter, allowing the original lower-level exception to be preserved for logging and diagnostics without leaking it to the caller.
- **Aggregation** — `ManagedExceptions` (plural) collects multiple `ManagedException` instances into a single raised exception, supporting bulk validation or multi-step failure scenarios.

---

### Exception Type Reference

| Exception Class            | Base Class             | Default Message                                                                 | Purpose                                                                                    |
|----------------------------|------------------------|---------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------|
| `ManagedException`         | `System.Exception`     | An internal application error has occurred.                                     | Root base class for all managed exceptions.                                                |
| `ManagedExceptions`        | `ManagedException`     | More than one error has occurred, see the `Exceptions` property for details.    | Aggregates multiple `ManagedException` instances into a single exception.                  |
| `CommunicationException`   | `ManagedException`     | A communication error has occurred within the application.                      | Raised when a communication failure occurs between application components or services.     |
| `ConfigurationException`   | `ManagedException`     | Configuration information for the application could not be found or is invalid. | Raised when application configuration is missing or contains invalid data.                 |
| `DataException`            | `ManagedException`     | An internal error occurred, data operation did not complete.                    | Raised when an error occurs while managing data.                                           |
| `LogicException`           | `ManagedException`     | The current operation had an internal error.                                    | Raised when an error occurs within application executing logic.                            |
| `SecurityException`        | `ManagedException`     | A security error has occurred within the application.                           | Raised when a security-related error is captured.                                          |
| `UnhandledException`       | `ManagedException`     | An internal application error has occurred. Please confirm the operation completed successfully. | Raised when an unhandled exception is caught and wrapped for safe propagation.  |
| `ValidationException`      | `ManagedException`     | A validation error has occurred, the data for `{field}` is missing or incomplete. | Raised when input data fails validation. Carries the optional `DataField` property.      |
| `AuthenticationException`  | `SecurityException`    | Access error has occurred the operation could not complete.                     | Raised specifically for authentication failures.                                           |
| `AuthorizationException`   | `SecurityException`    | Access error has occurred the operation could not complete.                     | Raised specifically for authorization/access-control failures.                             |
| `DataValidationException`  | `DataException`        | A validation error occurred with `{property}`.                                  | Raised when a specific property/field fails data validation. Carries the `PropertyName` property. |
| `DuplicateException`       | `DataException`        | The information is a duplicate of existing information, the operation could not complete. | Raised when duplicate data is detected.                                           |

---

### Additional Properties

| Exception Class           | Additional Property | Type     | Description                                              |
|---------------------------|---------------------|----------|----------------------------------------------------------|
| `ValidationException`     | `DataField`         | `string` | The name of the data field that failed validation.       |
| `DataValidationException` | `PropertyName`      | `string` | The name of the property where data validation failed.   |
| `ManagedExceptions`       | `Exceptions`        | `IReadOnlyList<ManagedException>` | The collection of all aggregated exceptions.  |

---

## Structured Logging Extensions

`LoggingExtensions` adds extension methods to `ILogger` that automatically capture caller member names and line numbers using `[CallerMemberName]` and `[CallerLineNumber]`, making structured log output more precise and diagnostics easier.

### Key Methods

| Method                          | Description                                                    |
|---------------------------------|----------------------------------------------------------------|
| `EnterLog`                      | Logs entry into a member at a specified log level              |
| `ExitLog`                       | Logs exit from a member at a specified log level               |
| Additional `ILogger` extensions | Enhanced logging with member and line number metadata          |

### Usage Example
```
public class OrderService { private readonly ILogger<OrderService> _logger;
{
    public OrderService(ILogger<OrderService> logger)
    {
        _logger = logger;
    }

    public async Task ProcessOrderAsync(int orderId)
    {
        _logger.EnterLog(LogLevel.Debug);

        try
        {
            // ... processing logic ...
        }
        catch (ManagedException ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
        finally
        {
            _logger.ExitLog(LogLevel.Debug);
        }
    }
}

```

Log output will automatically include the `MemberName` and `LineNumber` properties, enabling downstream structured log sinks (e.g., Serilog, Application Insights) to capture precise source location data.

---

## Dependency Injection Loader

`DependencyInjectionLoader` is an abstract base class that provides a standardized pattern for registering services at the library level. It supports a layered loading pipeline:

1. **`LoadLibraries`** — Load DI registrations from dependent child libraries (override to compose libraries).
2. **`LoadManualRegistration`** — Register services that require manual or conditional configuration (override as needed).
3. **`LoadRegistration`** — Primary service registrations for this library (override to register your services).

### Usage Example
```
public class MyLibraryLoader : DependencyInjectionLoader 
{ 

    protected override void LoadLibraries(IServiceCollection services, IConfiguration configuration)
    {
        // Load a dependent library's registrations
        new DependentLibraryLoader().Load(services, configuration);
    }

    protected override void LoadRegistration(IServiceCollection services, IConfiguration configuration) 
    { 
        services.AddScoped<IOrderService, OrderService>(); services.AddSingleton<IOrderRepository, OrderRepository>(); 
    }

}
```
**Registering in your application startup:**

```
var loader = new MyLibraryLoader(); 
loader.Load(builder.Services, builder.Configuration);

```

---

## Dependencies

| Package                                                  | Version |
|----------------------------------------------------------|---------|
| `Microsoft.Extensions.Configuration.Abstractions`       | 10.0.0  |
| `Microsoft.Extensions.DependencyInjection.Abstractions` | 10.0.0  |
| `Microsoft.Extensions.Logging.Abstractions`             | 10.0.0  |
| `System.Collections.Immutable` *(netstandard2.1 only)*  | 10.0.0  |

---

## Repository & Documentation

- **Source Code:** [https://github.com/CodeFactoryLLC/NDF](https://github.com/CodeFactoryLLC/NDF)
- **API Documentation:** [https://codefactoryllc.github.io/NDF](https://codefactoryllc.github.io/NDF)

---

## License

Copyright © 2026 CodeFactory, LLC. All rights reserved.

Licensed under the [MIT License](https://opensource.org/licenses/MIT).
