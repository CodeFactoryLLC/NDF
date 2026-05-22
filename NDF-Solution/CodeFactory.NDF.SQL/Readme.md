# CodeFactory.NDF.SQL

[![NuGet](https://img.shields.io/nuget/v/CodeFactory.NDF.SQL)](https://www.nuget.org/packages/CodeFactory.NDF.SQL)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

**Net Delivery Framework (NDF) — SQL Support Library**

`CodeFactory.NDF.SQL` is a supplemental library for the [Net Delivery Framework](https://github.com/CodeFactoryLLC/NDF) that provides structured MS SQL Server integration for .NET applications. It translates raw `SqlException` errors into the NDF managed exception hierarchy, giving your data layer consistent, consumer-friendly error handling with no boilerplate.

---

## 📦 Installation

Install via the NuGet Package Manager, the .NET CLI, or the Package Manager Console.

**.NET CLI**

```
dotnet add package CodeFactory.NDF.SQL
```

**Package Manager Console**

```
Install-Package CodeFactory.NDF.SQL
```

---

## 🛠 Usage

```csharp
using CodeFactory.NDF.SQL;
using System.Data.SqlClient;

public void ConnectToDatabase()
{
    try
    {
        // Your SQL Connection Logic Here
    }
    catch (SqlException ex)
    {
        // Translate and throw the managed exception
        ex.ThrowManagedException();
    }
}
```

---

### Exception Mapping Reference

The following SQL error numbers are translated automatically:

| SQL Error Number | SQL Condition                    | NDF Exception Thrown            |
|-----------------|----------------------------------|---------------------------------|
| `18456`         | Authentication failure           | `AuthenticationException`       |
| `18470`         | Account disabled                 | `AuthenticationException`       |
| `229`           | Permission denied (object)       | `AuthorizationException`        |
| `230`           | Permission denied (column)       | `AuthorizationException`        |
| `262`           | CREATE permission denied         | `AuthorizationException`        |
| `10060`         | Connection failure               | `CommunicationException`        |
| `53`            | Network path not found           | `CommunicationException`        |
| `10054`         | Connection forcibly closed       | `CommunicationException`        |
| `-2`            | Timeout                          | `CommunicationTimeoutException` |
| `4060`          | Cannot open database             | `ConfigurationException`        |
| `2601`          | Duplicate entry                  | `DuplicateException`            |
| `2627`          | Unique key constraint violation  | `DuplicateException`            |
| `515`           | Cannot insert NULL               | `DataValidationException`       |
| `8152`          | String/binary data truncation    | `DataValidationException`       |
| `547`           | Foreign key constraint violation | `ValidationException`           |
| `1205`          | Deadlock victim                  | `DataException`                 |
| *(all others)*  | Unknown SQL error                | `DataException`                 |

---

## 📚 API Reference

### `SqlExceptionManagement`

**Namespace:** `CodeFactory.NDF.SQL`

A static class containing extension methods and constants for SQL error management.

#### Extension Methods

| Method | Description |
|--------|-------------|
| `ThrowManagedException(this SqlException source)` | Evaluates the `SqlException` error number and throws the corresponding NDF managed exception. |

#### Error Number Constants

All known SQL error numbers are exposed as `public const int` fields for use in your own switch statements or logging:

- `ConnectionFailureNumber`, `NetworkPathNotFoundNumber`, `ConnectionResetNumber`
- `TimeoutNumber`
- `AuthenticationErrorNumber`, `AccountDisabledNumber`
- `PermissionDeniedNumber`, `ColumnPermissionDeniedNumber`, `CreatePermissionDeniedNumber`
- `CannotOpenDatabaseNumber`
- `CannotInsertNullNumber`, `ForeignKeyConstraintNumber`, `DataTruncationNumber`
- `DeadlockNumber`
- `DuplicateEntryNumber`, `UniqueKeyConstraintViolationNumber`

---

## 🔗 Dependencies

| Package | Version |
|---------|---------|
| `CodeFactory.NDF` | *(project reference — same version)* |
| `Microsoft.Data.SqlClient` | `7.0.1` |

---

## 📋 Requirements

- **.NET 10** or later

> **Note:** Support for .NET Standard 2.1 has been removed as of version `10.x`. Use an earlier version of this package if .NET Standard 2.1 support is required.

---

## 📄 License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).  
Copyright © 2026 CodeFactory, LLC.

---

## 🏢 About CodeFactory, LLC.

**CodeFactory, LLC.** builds developer tooling and frameworks that accelerate enterprise .NET application delivery.  
Visit us at [https://github.com/CodeFactoryLLC/NDF](https://github.com/CodeFactoryLLC/NDF).