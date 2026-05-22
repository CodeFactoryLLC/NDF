Here is a comprehensive summary of the **NDF (Net Delivery Framework)** solution:

---

# Net Delivery Framework (NDF) — Solution Summary

**Repository:** [https://github.com/CodeFactoryLLC/NDF](https://github.com/CodeFactoryLLC/NDF)
**Author:** CodeFactory, LLC. | **License:** MIT | **Version:** 10.26141.1

---

## Solution Overview

The NDF solution provides reusable delivery framework patterns for .NET solutions. It is composed of two NuGet-published class library projects that implement consistent exception handling, structured logging, and dependency injection patterns.

---

## Projects

### 1. `CodeFactory.NDF` — Core Library

**NuGet:** `CodeFactory.NDF`
**Target Frameworks:** `.NET Standard 2.1`, `.NET 10.0`

The foundational library for the framework. All other NDF libraries depend on this package.

#### Features

##### Managed Exception Hierarchy
A structured, application-safe exception system built on a common `ManagedException` base class. Exceptions surface user-friendly messages without leaking internal details.

| Exception Class | Base Class | Purpose |
|---|---|---|
| `ManagedException` | `System.Exception` | Root base for all managed exceptions |
| `ManagedExceptions` | `ManagedException` | Aggregates multiple exceptions into one |
| `CommunicationException` | `ManagedException` | Communication failures between services |
| `ConfigurationException` | `ManagedException` | Missing or invalid configuration |
| `DataException` | `ManagedException` | Data operation errors |
| `LogicException` | `ManagedException` | Application logic errors |
| `SecurityException` | `ManagedException` | Security-related errors |
| `UnhandledException` | `ManagedException` | Wraps unexpected exceptions safely |
| `ValidationException` | `ManagedException` | Input data validation failures (+ `DataField` property) |
| `AuthenticationException` | `SecurityException` | Authentication failures |
| `AuthorizationException` | `SecurityException` | Authorization/access-control failures |
| `DataValidationException` | `DataException` | Property-level data validation (+ `PropertyName` property) |
| `DuplicateException` | `DataException` | Duplicate data detection |

##### Structured Logging Extensions (`LoggingExtensions`)
Extension methods on `ILogger` that automatically inject caller metadata (member name, line number) into log output via `[CallerMemberName]` and `[CallerLineNumber]`.

| Method | Description |
|---|---|
| `EnterLog` | Logs entry into a member at the specified log level |
| `ExitLog` | Logs exit from a member at the specified log level |

Compatible with structured log sinks such as Serilog and Application Insights.

##### Dependency Injection Loader (`DependencyInjectionLoader`)
An abstract base class for library-level DI registration. Supports a layered, composable loading pipeline:

| Override Method | Purpose |
|---|---|
| `LoadLibraries` | Load registrations from dependent child libraries |
| `LoadManualRegistration` | Register services needing manual/conditional setup |
| `LoadRegistration` | Primary service registrations for the library |

#### Dependencies

| Package | Version |
|---|---|
| `Microsoft.Extensions.Configuration.Abstractions` | 10.0.0 |
| `Microsoft.Extensions.DependencyInjection.Abstractions` | 10.0.0 |
| `Microsoft.Extensions.Logging.Abstractions` | 10.0.0 |
| `System.Collections.Immutable` *(netstandard2.1 only)* | 10.0.0 |

---

### 2. `CodeFactory.NDF.SQL` — SQL Support Library

**NuGet:** `CodeFactory.NDF.SQL`
**Target Framework:** `.NET 10.0` only

A supplemental library providing structured MS SQL Server integration. Translates raw `SqlException` errors into the NDF managed exception hierarchy — eliminating boilerplate data-layer error handling.

#### Features

##### SQL Exception Management (`SqlExceptionManagement`)
A static class with extension methods that map `SqlException` error numbers to the appropriate NDF managed exception.

| Method | Description |
|---|---|
| `ThrowManagedException(this SqlException source)` | Evaluates the SQL error number and throws the corresponding NDF managed exception |

##### SQL Error-to-Exception Mapping

| SQL Error | Condition | NDF Exception |
|---|---|---|
| `18456`, `18470` | Auth failure / Account disabled | `AuthenticationException` |
| `229`, `230`, `262` | Permission denied | `AuthorizationException` |
| `10060`, `53`, `10054` | Connection/network failure | `CommunicationException` |
| `-2` | Timeout | `CommunicationTimeoutException` |
| `4060` | Cannot open database | `ConfigurationException` |
| `2601`, `2627` | Duplicate / unique key violation | `DuplicateException` |
| `515`, `8152` | NULL insert / data truncation | `DataValidationException` |
| `547` | Foreign key constraint | `ValidationException` |
| `1205` | Deadlock victim | `DataException` |
| *(all others)* | Unknown SQL error | `DataException` |

##### SQL Error Number Constants
All known SQL error numbers are exposed as `public const int` fields (e.g., `ConnectionFailureNumber`, `TimeoutNumber`, `DeadlockNumber`) for use in custom switch statements or logging.

#### Dependencies

| Package | Version |
|---|---|
| `CodeFactory.NDF` | *(project reference)* |
| `Microsoft.Data.SqlClient` | `7.0.1` |

> ⚠️ `.NET Standard 2.1` support was removed in version `10.x`. Use an earlier version if Standard 2.1 is required.

---

## Solution Structure

```
NDF-Solution/
├── CodeFactory.NDF/            # Core framework library (netstandard2.1 + net10.0)
│   ├── ManagedException.cs
│   ├── LoggingExtensions.cs
│   ├── DependencyInjectionLoader.cs
│   └── StandardExceptionMessages.resx
├── CodeFactory.NDF.SQL/        # SQL integration library (net10.0)
│   └── SqlExceptionManagement.cs
└── NDF-Solution.sln
```

---

## API Documentation

Full API docs are published at: [https://codefactoryllc.github.io/NDF](https://codefactoryllc.github.io/NDF)


