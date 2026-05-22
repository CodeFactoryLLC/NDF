#CodeFactory.NDF.SQL# CodeFactory.NDF.SQL

![NDF Logo](NDF128.png)

**Net Delivery Framework (NDF) – SQL Support Library**  
*by [CodeFactory, LLC](https://github.com/CodeFactoryLLC/NDF)*

[![NuGet](https://img.shields.io/nuget/v/CodeFactory.NDF.SQL)](https://www.nuget.org/packages/CodeFactory.NDF.SQL)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![Target Framework](https://img.shields.io/badge/.NET-10.0-purple)](https://dotnet.microsoft.com/)

---

## Overview

`CodeFactory.NDF.SQL` extends the [CodeFactory.NDF](https://www.nuget.org/packages/CodeFactory.NDF) core library with Microsoft SQL Server integration support. It provides structured, opinionated exception management that translates raw `SqlException` error numbers into strongly-typed NDF managed exceptions, keeping your application layer clean and free of SQL-specific error handling logic.

---

## Features

- **SQL Exception Translation** — Automatically maps well-known `SqlException` error numbers to the appropriate NDF managed exceptions.
- **Strongly-Typed Exceptions** — Converts raw SQL errors into NDF exceptions such as `AuthenticationException`, `AuthorizationException`, `CommunicationException`, `CommunicationTimeoutException`, `ConfigurationException`, `DataValidationException`, `DuplicateException`, and `DataException`.
- **Extension Method Design** — Clean and intuitive API via a single extension method on `SqlException`.
- **Covers Common SQL Error Scenarios** — Handles connection failures, timeouts, authentication errors, permission violations, NULL constraint violations, data truncation, foreign key violations, deadlocks, duplicate key errors, and more.

---

## Requirements

| Dependency | Version |
|---|---|
| .NET | 10.0 |
| [CodeFactory.NDF](https://www.nuget.org/packages/CodeFactory.NDF) | 10.x |
| [Microsoft.Data.SqlClient](https://www.nuget.org/packages/Microsoft.Data.SqlClient) | 7.0.1+ |

---

## Installation

Install via the .NET CLI:




````````
### Exception Mapping

The following SQL error numbers are handled and translated:

| SQL Error # | Scenario | NDF Exception Raised |
|-------------|----------|---------------------|
| 18456       | Authentication failure      | `AuthenticationException` |
| 18470       | Account disabled           | `AuthenticationException` |
| 229         | Object permission denied    | `AuthorizationException` |
| 230         | Column permission denied    | `AuthorizationException` |
| 262         | CREATE permission denied    | `AuthorizationException` |
| 10060       | Connection failure          | `CommunicationException` |
| 53          | Network path not found      | `CommunicationException` |
| 10054       | Connection reset by remote host | `CommunicationException` |
| -2          | Query timeout               | `CommunicationTimeoutException` |
| 4060        | Cannot open database        | `ConfigurationException` |
| 515         | Cannot insert NULL          | `DataValidationException` |
| 8152        | String/binary data truncation | `DataValidationException` |
| 547         | Foreign key constraint violation | `ValidationException` |
| 2601        | Duplicate entry (unique index) | `DuplicateException` |
| 2627        | Unique key constraint violation | `DuplicateException` |
| 1205        | Deadlock victim             | `DataException` |
| *(all others)* | Unhandled SQL error      | `DataException` |

---

## Related Packages

| Package | Description |
|---|---|
| [CodeFactory.NDF](https://www.nuget.org/packages/CodeFactory.NDF) | Core Net Delivery Framework — base exceptions, patterns, and DI support |

---

## License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).  
Copyright © 2026 CodeFactory, LLC.

---

## Repository

Source code and issue tracking:  
[https://github.com/CodeFactoryLLC/NDF](https://github.com/CodeFactoryLLC/NDF)


