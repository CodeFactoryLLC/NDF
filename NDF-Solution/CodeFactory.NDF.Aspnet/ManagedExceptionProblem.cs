//*****************************************************************************
//* Net Delivery Framework
//* Copyright (c) 2026 CodeFactory, LLC
//*****************************************************************************

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace CodeFactory.NDF.Aspnet
{
    /// <summary>
    /// Converts NDF <see cref="ManagedException"/> types to <see cref="ProblemDetails"/> and back.
    /// Inner exceptions, stack traces, and other <see cref="Exception"/> runtime data are never mapped.
    /// </summary>
    public static class ManagedExceptionProblem
    {
        /// <summary>
        /// Absolute URI prefix written to <see cref="ProblemDetails.Type"/>. The managed exception type name is appended.
        /// </summary>
        public const string TypeUriBase = "https://codefactoryllc.github.io/NDF/problems/";

        /// <summary>
        /// Extension key used for the nested problem array produced from <see cref="ManagedExceptions"/>.
        /// </summary>
        public const string ExceptionsExtensionKey = "exceptions";

        /// <summary>
        /// Extension key used for <see cref="ValidationException.DataField"/>.
        /// </summary>
        public const string DataFieldExtensionKey = "dataField";

        /// <summary>
        /// Extension key used for <see cref="DataValidationException.PropertyName"/>.
        /// </summary>
        public const string PropertyNameExtensionKey = "propertyName";

        /// <summary>
        /// Maximum nesting depth converted for <see cref="ManagedExceptions"/>. Deeper children are omitted.
        /// </summary>
        public const int MaxExceptionDepth = 8;

        /// <summary>
        /// Maximum number of child exceptions converted at each <see cref="ManagedExceptions"/> level. Extra children are omitted.
        /// </summary>
        public const int MaxExceptionChildren = 64;

        /// <summary>HTTP status used for <see cref="ManagedException"/>.</summary>
        public const int ManagedExceptionStatus = 480;

        /// <summary>HTTP status used for <see cref="ManagedExceptions"/>.</summary>
        public const int ManagedExceptionsStatus = 481;

        /// <summary>HTTP status used for <see cref="CommunicationException"/>.</summary>
        public const int CommunicationExceptionStatus = 482;

        /// <summary>HTTP status used for <see cref="CommunicationTimeoutException"/>.</summary>
        public const int CommunicationTimeoutExceptionStatus = 483;

        /// <summary>HTTP status used for <see cref="ConfigurationException"/>.</summary>
        public const int ConfigurationExceptionStatus = 484;

        /// <summary>HTTP status used for <see cref="DataException"/>.</summary>
        public const int DataExceptionStatus = 485;

        /// <summary>HTTP status used for <see cref="DataValidationException"/>.</summary>
        public const int DataValidationExceptionStatus = 486;

        /// <summary>HTTP status used for <see cref="DuplicateException"/>.</summary>
        public const int DuplicateExceptionStatus = 487;

        /// <summary>HTTP status used for <see cref="LogicException"/>.</summary>
        public const int LogicExceptionStatus = 488;

        /// <summary>HTTP status used for <see cref="SecurityException"/>.</summary>
        public const int SecurityExceptionStatus = 489;

        /// <summary>HTTP status used for <see cref="AuthenticationException"/>.</summary>
        public const int AuthenticationExceptionStatus = 490;

        /// <summary>HTTP status used for <see cref="AuthorizationException"/>.</summary>
        public const int AuthorizationExceptionStatus = 491;

        /// <summary>HTTP status used for <see cref="UnhandledException"/>.</summary>
        public const int UnhandledExceptionStatus = 492;

        /// <summary>HTTP status used for <see cref="ValidationException"/>.</summary>
        public const int ValidationExceptionStatus = 493;

        /// <summary>Title used for <see cref="ManagedException"/>.</summary>
        public const string ManagedExceptionTitle = "Application error";

        /// <summary>Title used for <see cref="ManagedExceptions"/>.</summary>
        public const string ManagedExceptionsTitle = "Multiple errors";

        /// <summary>Title used for <see cref="CommunicationException"/>.</summary>
        public const string CommunicationExceptionTitle = "Communication error";

        /// <summary>Title used for <see cref="CommunicationTimeoutException"/>.</summary>
        public const string CommunicationTimeoutExceptionTitle = "Communication timeout";

        /// <summary>Title used for <see cref="ConfigurationException"/>.</summary>
        public const string ConfigurationExceptionTitle = "Configuration error";

        /// <summary>Title used for <see cref="DataException"/>.</summary>
        public const string DataExceptionTitle = "Data error";

        /// <summary>Title used for <see cref="DataValidationException"/>.</summary>
        public const string DataValidationExceptionTitle = "Data validation error";

        /// <summary>Title used for <see cref="DuplicateException"/>.</summary>
        public const string DuplicateExceptionTitle = "Duplicate data";

        /// <summary>Title used for <see cref="LogicException"/>.</summary>
        public const string LogicExceptionTitle = "Logic error";

        /// <summary>Title used for <see cref="SecurityException"/>.</summary>
        public const string SecurityExceptionTitle = "Security error";

        /// <summary>Title used for <see cref="AuthenticationException"/>.</summary>
        public const string AuthenticationExceptionTitle = "Authentication error";

        /// <summary>Title used for <see cref="AuthorizationException"/>.</summary>
        public const string AuthorizationExceptionTitle = "Authorization error";

        /// <summary>Title used for <see cref="UnhandledException"/>.</summary>
        public const string UnhandledExceptionTitle = "Unhandled error";

        /// <summary>Title used for <see cref="ValidationException"/>.</summary>
        public const string ValidationExceptionTitle = "Validation error";

        private static readonly JsonSerializerOptions ProblemJsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private static readonly ConcurrentDictionary<Type, ExceptionMap> KnownTypeCache = new();

        private static readonly FrozenDictionary<Type, ExceptionMap> ForwardMap = CreateForwardMap();

        private static readonly FrozenDictionary<string, Func<ProblemDetails, ManagedException>> ReverseMap = CreateReverseMap();

        private static FrozenDictionary<Type, ExceptionMap> CreateForwardMap()
        {
            return new Dictionary<Type, ExceptionMap>
            {
                [typeof(ManagedException)] = CreateMap(ManagedExceptionStatus, ManagedExceptionTitle, nameof(ManagedException)),
                [typeof(ManagedExceptions)] = CreateMap(ManagedExceptionsStatus, ManagedExceptionsTitle, nameof(ManagedExceptions)),
                [typeof(CommunicationException)] = CreateMap(CommunicationExceptionStatus, CommunicationExceptionTitle, nameof(CommunicationException)),
                [typeof(CommunicationTimeoutException)] = CreateMap(CommunicationTimeoutExceptionStatus, CommunicationTimeoutExceptionTitle, nameof(CommunicationTimeoutException)),
                [typeof(ConfigurationException)] = CreateMap(ConfigurationExceptionStatus, ConfigurationExceptionTitle, nameof(ConfigurationException)),
                [typeof(DataException)] = CreateMap(DataExceptionStatus, DataExceptionTitle, nameof(DataException)),
                [typeof(DataValidationException)] = CreateMap(DataValidationExceptionStatus, DataValidationExceptionTitle, nameof(DataValidationException)),
                [typeof(DuplicateException)] = CreateMap(DuplicateExceptionStatus, DuplicateExceptionTitle, nameof(DuplicateException)),
                [typeof(LogicException)] = CreateMap(LogicExceptionStatus, LogicExceptionTitle, nameof(LogicException)),
                [typeof(SecurityException)] = CreateMap(SecurityExceptionStatus, SecurityExceptionTitle, nameof(SecurityException)),
                [typeof(AuthenticationException)] = CreateMap(AuthenticationExceptionStatus, AuthenticationExceptionTitle, nameof(AuthenticationException)),
                [typeof(AuthorizationException)] = CreateMap(AuthorizationExceptionStatus, AuthorizationExceptionTitle, nameof(AuthorizationException)),
                [typeof(UnhandledException)] = CreateMap(UnhandledExceptionStatus, UnhandledExceptionTitle, nameof(UnhandledException)),
                [typeof(ValidationException)] = CreateMap(ValidationExceptionStatus, ValidationExceptionTitle, nameof(ValidationException))
            }.ToFrozenDictionary();
        }

        private static FrozenDictionary<string, Func<ProblemDetails, ManagedException>> CreateReverseMap()
        {
            return new Dictionary<string, Func<ProblemDetails, ManagedException>>(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(ManagedException)] = static problem => CreateWithMessage(static message => new ManagedException(message), static () => new ManagedException(), problem),
                [nameof(CommunicationException)] = static problem => CreateWithMessage(static message => new CommunicationException(message), static () => new CommunicationException(), problem),
                [nameof(CommunicationTimeoutException)] = static problem => CreateWithMessage(static message => new CommunicationTimeoutException(message), static () => new CommunicationTimeoutException(), problem),
                [nameof(ConfigurationException)] = static problem => CreateWithMessage(static message => new ConfigurationException(message), static () => new ConfigurationException(), problem),
                [nameof(DataException)] = static problem => CreateWithMessage(static message => new DataException(message), static () => new DataException(), problem),
                [nameof(DataValidationException)] = CreateDataValidationException,
                [nameof(DuplicateException)] = static problem => CreateWithMessage(static message => new DuplicateException(message), static () => new DuplicateException(), problem),
                [nameof(LogicException)] = static problem => CreateWithMessage(static message => new LogicException(message), static () => new LogicException(), problem),
                [nameof(SecurityException)] = static problem => CreateWithMessage(static message => new SecurityException(message), static () => new SecurityException(), problem),
                [nameof(AuthenticationException)] = static problem => CreateWithMessage(static message => new AuthenticationException(message), static () => new AuthenticationException(), problem),
                [nameof(AuthorizationException)] = static problem => CreateWithMessage(static message => new AuthorizationException(message), static () => new AuthorizationException(), problem),
                [nameof(UnhandledException)] = static problem => CreateWithMessage(static message => new UnhandledException(message), static () => new UnhandledException(), problem),
                [nameof(ValidationException)] = CreateValidationException
            }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
        }

        private static ExceptionMap CreateMap(int status, string title, string typeName) =>
            new(status, title, typeName, TypeUriBase + typeName);

        /// <summary>
        /// Creates a <see cref="ProblemDetails"/> representation of <paramref name="exception"/>.
        /// Nested <see cref="ManagedExceptions"/> are written as a hierarchy under the <c>exceptions</c> extension.
        /// </summary>
        /// <param name="exception">The managed exception to convert.</param>
        /// <returns>A problem details document for the exception.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="exception"/> is null.</exception>
        public static ProblemDetails CreateProblemDetails(ManagedException exception)
        {
            ArgumentNullException.ThrowIfNull(exception);

            return CreateProblemDetails(exception, 0);
        }

        /// <summary>
        /// Creates a <see cref="ProblemDetails"/> representation of <paramref name="exception"/> at <paramref name="depth"/>.
        /// </summary>
        /// <param name="exception">The managed exception to convert.</param>
        /// <param name="depth">Current nesting depth for <see cref="ManagedExceptions"/>.</param>
        /// <returns>A problem details document for the exception.</returns>
        private static ProblemDetails CreateProblemDetails(ManagedException exception, int depth)
        {
            var map = GetMap(exception.GetType());

            var problem = new ProblemDetails
            {
                Type = map.TypeUri,
                Title = map.Title,
                Status = map.Status,
                Detail = exception.Message
            };

            if (exception is ManagedExceptions aggregate && depth < MaxExceptionDepth)
            {
                var source = aggregate.Exceptions;
                var count = source.Count < MaxExceptionChildren ? source.Count : MaxExceptionChildren;
                var children = new List<ProblemDetails>(count);
                for (var index = 0; index < count; index++)
                {
                    children.Add(CreateProblemDetails(source[index], depth + 1));
                }

                problem.Extensions[ExceptionsExtensionKey] = children;
            }
            else if (exception is ValidationException validation)
            {
                if (!string.IsNullOrEmpty(validation.DataField))
                {
                    problem.Extensions[DataFieldExtensionKey] = validation.DataField;
                }
            }
            else if (exception is DataValidationException dataValidation)
            {
                if (dataValidation.PropertyName != null)
                {
                    problem.Extensions[PropertyNameExtensionKey] = dataValidation.PropertyName;
                }
            }

            return problem;
        }

        /// <summary>
        /// Recreates the managed exception represented by <paramref name="problemDetails"/>.
        /// Nested problem arrays under <c>exceptions</c> are converted back to <see cref="ManagedExceptions"/>.
        /// Inner exceptions are not restored.
        /// </summary>
        /// <param name="problemDetails">The problem details document to convert.</param>
        /// <returns>The managed exception identified by <see cref="ProblemDetails.Type"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="problemDetails"/> is null.</exception>
        public static ManagedException CreateManagedException(ProblemDetails problemDetails)
        {
            ArgumentNullException.ThrowIfNull(problemDetails);

            return CreateManagedException(problemDetails, 0);
        }

        /// <summary>
        /// Recreates the managed exception represented by <paramref name="problemDetails"/> at <paramref name="depth"/>.
        /// </summary>
        /// <param name="problemDetails">The problem details document to convert.</param>
        /// <param name="depth">Current nesting depth for <see cref="ManagedExceptions"/>.</param>
        /// <returns>The managed exception identified by <see cref="ProblemDetails.Type"/>.</returns>
        private static ManagedException CreateManagedException(ProblemDetails problemDetails, int depth)
        {
            var typeName = TryGetTypeName(problemDetails.Type);
            if (typeName != null)
            {
                if (typeName.Equals(nameof(ManagedExceptions), StringComparison.OrdinalIgnoreCase))
                {
                    return CreateManagedExceptions(problemDetails, depth);
                }

                if (ReverseMap.TryGetValue(typeName, out var factory))
                {
                    return factory(problemDetails);
                }
            }

            return CreateWithMessage(static message => new ManagedException(message), static () => new ManagedException(), problemDetails);
        }

        /// <summary>
        /// Resolves <paramref name="exceptionType"/> to the closest mapped NDF exception map.
        /// Walks the type hierarchy until a <see cref="ForwardMap"/> key is found and stops at <see cref="Exception"/>.
        /// Results are cached by runtime type.
        /// </summary>
        /// <param name="exceptionType">The runtime exception type to resolve.</param>
        /// <returns>
        /// The mapped entry when one is found; otherwise the <see cref="ManagedException"/> entry.
        /// </returns>
        private static ExceptionMap GetMap(Type exceptionType)
        {
            return KnownTypeCache.GetOrAdd(exceptionType, static type =>
            {
                for (var current = type; current != null; current = current.BaseType)
                {
                    if (ForwardMap.TryGetValue(current, out var map))
                    {
                        return map;
                    }

                    if (current == typeof(Exception))
                    {
                        break;
                    }
                }

                return ForwardMap[typeof(ManagedException)];
            });
        }

        /// <summary>
        /// Extracts the managed exception type name from a <see cref="ProblemDetails.Type"/> value.
        /// Prefers a suffix after <see cref="TypeUriBase"/>; otherwise uses the last path segment or the trimmed value.
        /// </summary>
        /// <param name="type">The type URI or type name to parse.</param>
        /// <returns>
        /// The extracted type name, or <c>null</c> when <paramref name="type"/> is null, empty, or whitespace.
        /// </returns>
        private static string? TryGetTypeName(string? type)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                return null;
            }

            type = type.Trim();

            if (type.StartsWith(TypeUriBase, StringComparison.OrdinalIgnoreCase))
            {
                type = type[TypeUriBase.Length..];
            }
            else
            {
                var lastSlash = type.LastIndexOf('/');
                if (lastSlash >= 0 && lastSlash < type.Length - 1)
                {
                    type = type[(lastSlash + 1)..];
                }
            }

            type = type.Trim().TrimEnd('/');
            return string.IsNullOrWhiteSpace(type) ? null : type;
        }

        /// <summary>
        /// Creates a managed exception using <see cref="ProblemDetails.Detail"/> when it is present.
        /// </summary>
        /// <param name="withMessage">Factory that creates the exception with a message.</param>
        /// <param name="defaultConstructor">Factory that creates the exception with the default message.</param>
        /// <param name="problem">The problem details document that supplies the message.</param>
        /// <returns>
        /// The exception created by <paramref name="withMessage"/> when <see cref="ProblemDetails.Detail"/> is not empty;
        /// otherwise the exception created by <paramref name="defaultConstructor"/>.
        /// </returns>
        private static ManagedException CreateWithMessage(
            Func<string, ManagedException> withMessage,
            Func<ManagedException> defaultConstructor,
            ProblemDetails problem)
        {
            return string.IsNullOrEmpty(problem.Detail) ? defaultConstructor() : withMessage(problem.Detail);
        }

        /// <summary>
        /// Recreates a <see cref="ValidationException"/> from <paramref name="problem"/>.
        /// Reads the data field from the <c>dataField</c> or <c>DataField</c> extension.
        /// </summary>
        /// <param name="problem">The problem details document to convert.</param>
        /// <returns>
        /// A <see cref="ValidationException"/> using <see cref="ProblemDetails.Detail"/> when present and the restored data field.
        /// </returns>
        private static ManagedException CreateValidationException(ProblemDetails problem)
        {
            var dataField = GetExtensionString(problem, DataFieldExtensionKey, "DataField");
            if (string.IsNullOrEmpty(problem.Detail))
            {
                return dataField is null ? new ValidationException() : new ValidationException(dataField);
            }

            return new ValidationException(problem.Detail, dataField ?? string.Empty);
        }

        /// <summary>
        /// Recreates a <see cref="DataValidationException"/> from <paramref name="problem"/>.
        /// Reads the property name from the <c>propertyName</c> or <c>PropertyName</c> extension and defaults to an empty string.
        /// </summary>
        /// <param name="problem">The problem details document to convert.</param>
        /// <returns>
        /// A <see cref="DataValidationException"/> using the restored property name and <see cref="ProblemDetails.Detail"/> when present.
        /// </returns>
        private static ManagedException CreateDataValidationException(ProblemDetails problem)
        {
            var propertyName = GetExtensionString(problem, PropertyNameExtensionKey, "PropertyName") ?? string.Empty;
            return string.IsNullOrEmpty(problem.Detail)
                ? new DataValidationException(propertyName)
                : new DataValidationException(propertyName, problem.Detail);
        }

        /// <summary>
        /// Recreates a <see cref="ManagedExceptions"/> aggregate from nested problem documents.
        /// Stops at <see cref="MaxExceptionDepth"/> and <see cref="MaxExceptionChildren"/>.
        /// </summary>
        /// <param name="problem">The problem details document that may contain an <c>exceptions</c> extension.</param>
        /// <param name="depth">Current nesting depth for the aggregate.</param>
        /// <returns>
        /// A <see cref="ManagedExceptions"/> instance whose children are created from nested problem documents.
        /// </returns>
        private static ManagedException CreateManagedExceptions(ProblemDetails problem, int depth)
        {
            if (depth >= MaxExceptionDepth)
            {
                return new ManagedExceptions(Array.Empty<ManagedException>());
            }

            var nested = GetNestedProblems(problem);
            var count = nested.Count < MaxExceptionChildren ? nested.Count : MaxExceptionChildren;
            var children = new List<ManagedException>(count);
            for (var index = 0; index < count; index++)
            {
                children.Add(CreateManagedException(nested[index], depth + 1));
            }

            return new ManagedExceptions(children);
        }

        /// <summary>
        /// Reads a string extension value from <paramref name="problem"/> using camel-case then Pascal-case keys.
        /// </summary>
        /// <param name="problem">The problem details document that contains extensions.</param>
        /// <param name="camelCaseKey">The preferred camel-case extension key.</param>
        /// <param name="pascalCaseKey">The fallback Pascal-case extension key.</param>
        /// <returns>
        /// The extension value converted to a string, or <c>null</c> when the key is missing or the value cannot be converted.
        /// </returns>
        private static string? GetExtensionString(ProblemDetails problem, string camelCaseKey, string pascalCaseKey)
        {
            if (!TryGetExtension(problem, camelCaseKey, pascalCaseKey, out var value))
            {
                return null;
            }

            return ConvertToString(value);
        }

        /// <summary>
        /// Attempts to read an extension from <paramref name="problem"/> using camel-case then Pascal-case keys.
        /// </summary>
        /// <param name="problem">The problem details document that contains extensions.</param>
        /// <param name="camelCaseKey">The preferred camel-case extension key.</param>
        /// <param name="pascalCaseKey">The fallback Pascal-case extension key.</param>
        /// <param name="value">When this method returns, the extension value if found; otherwise <c>null</c>.</param>
        /// <returns><c>true</c> when either key is present; otherwise <c>false</c>.</returns>
        private static bool TryGetExtension(ProblemDetails problem, string camelCaseKey, string pascalCaseKey, out object? value)
        {
            if (problem.Extensions.TryGetValue(camelCaseKey, out value))
            {
                return true;
            }

            return problem.Extensions.TryGetValue(pascalCaseKey, out value);
        }

        /// <summary>
        /// Converts an extension value to a string, including <see cref="JsonElement"/> string and non-string kinds.
        /// </summary>
        /// <param name="value">The extension value to convert.</param>
        /// <returns>
        /// The string representation of <paramref name="value"/>, or <c>null</c> when the value is null or a JSON null/undefined element.
        /// </returns>
        private static string? ConvertToString(object? value)
        {
            return value switch
            {
                null => null,
                string text => text,
                JsonElement { ValueKind: JsonValueKind.String } element => element.GetString(),
                JsonElement { ValueKind: JsonValueKind.Null or JsonValueKind.Undefined } => null,
                JsonElement element => element.ToString(),
                _ => value.ToString()
            };
        }

        /// <summary>
        /// Reads nested problem documents from the <c>exceptions</c> or <c>Exceptions</c> extension.
        /// </summary>
        /// <param name="problem">The aggregate problem details document.</param>
        /// <returns>
        /// The nested problem documents, or an empty list when the extension is missing or null.
        /// </returns>
        private static IReadOnlyList<ProblemDetails> GetNestedProblems(ProblemDetails problem)
        {
            if (!TryGetExtension(problem, ExceptionsExtensionKey, "Exceptions", out var value) || value is null)
            {
                return Array.Empty<ProblemDetails>();
            }

            return NormalizeProblemList(value);
        }

        /// <summary>
        /// Normalizes an extension value into a list of <see cref="ProblemDetails"/> documents.
        /// Accepts <see cref="IEnumerable{T}"/> of <see cref="ProblemDetails"/>, <see cref="JsonElement"/>, other enumerables, or a single object. Raw strings are ignored.
        /// </summary>
        /// <param name="value">The extension value that may contain nested problems.</param>
        /// <returns>The converted problem documents that could be parsed from <paramref name="value"/>.</returns>
        private static List<ProblemDetails> NormalizeProblemList(object value)
        {
            var result = new List<ProblemDetails>();

            switch (value)
            {
                case IEnumerable<ProblemDetails> problems:
                    result.AddRange(problems.Where(problem => problem != null));
                    break;

                case JsonElement element:
                    AddFromJsonElement(result, element);
                    break;

                case string:
                    break;

                case IEnumerable enumerable:
                    foreach (var item in enumerable)
                    {
                        var problem = ConvertToProblemDetails(item);
                        if (problem != null)
                        {
                            result.Add(problem);
                        }
                    }
                    break;

                default:
                    var single = ConvertToProblemDetails(value);
                    if (single != null)
                    {
                        result.Add(single);
                    }
                    break;
            }

            return result;
        }

        /// <summary>
        /// Adds problem documents from a JSON array or object to <paramref name="result"/>.
        /// Arrays are processed recursively; objects are deserialized as <see cref="ProblemDetails"/>.
        /// </summary>
        /// <param name="result">The list that receives converted problem documents.</param>
        /// <param name="element">The JSON element to convert.</param>
        private static void AddFromJsonElement(List<ProblemDetails> result, JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Array:
                    foreach (var item in element.EnumerateArray())
                    {
                        if (result.Count >= MaxExceptionChildren)
                        {
                            break;
                        }

                        AddFromJsonElement(result, item);
                    }
                    break;

                case JsonValueKind.Object:
                    if (result.Count >= MaxExceptionChildren)
                    {
                        break;
                    }

                    var problem = element.Deserialize<ProblemDetails>(ProblemJsonOptions);
                    if (problem != null)
                    {
                        result.Add(problem);
                    }
                    break;
            }
        }

        /// <summary>
        /// Converts a nested extension item to <see cref="ProblemDetails"/>.
        /// Existing problem documents are returned as-is; JSON objects are deserialized. Other types are ignored.
        /// </summary>
        /// <param name="item">The nested value to convert.</param>
        /// <returns>
        /// The converted <see cref="ProblemDetails"/>, or <c>null</c> when <paramref name="item"/> is null or not a problem document.
        /// </returns>
        private static ProblemDetails? ConvertToProblemDetails(object? item)
        {
            return item switch
            {
                ProblemDetails problem => problem,
                JsonElement { ValueKind: JsonValueKind.Object } element => element.Deserialize<ProblemDetails>(ProblemJsonOptions),
                _ => null
            };
        }

        private readonly record struct ExceptionMap(int Status, string Title, string TypeName, string TypeUri);
    }
}
