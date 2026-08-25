//*****************************************************************************
//* Net Delivery Framework
//* Copyright (c) 2026 CodeFactory, LLC
//*****************************************************************************

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace CodeFactory.NDF.Aspnet.Tests
{
    /// <summary>
    /// Tests for <see cref="ManagedExceptionProblem"/> conversion limits and round-trips.
    /// </summary>
    public class ManagedExceptionProblemTests
    {
        [Theory]
        [MemberData(nameof(MappedExceptions))]
        public void CreateProblemDetails_ThenCreateManagedException_RoundTripsTypeAndMessage(ManagedException exception)
        {
            var problem = ManagedExceptionProblem.CreateProblemDetails(exception);
            var restored = ManagedExceptionProblem.CreateManagedException(problem);

            Assert.Equal(exception.GetType(), restored.GetType());
            Assert.Equal(exception.Message, restored.Message);
            Assert.StartsWith(ManagedExceptionProblem.TypeUriBase, problem.Type);
            Assert.InRange(problem.Status.GetValueOrDefault(), 480, 493);
        }

        public static TheoryData<ManagedException> MappedExceptions =>
        [
            new ManagedException("application"),
            new CommunicationException("communication"),
            new CommunicationTimeoutException("timeout"),
            new ConfigurationException("configuration"),
            new DataException("data"),
            new DataValidationException("Amount", "data validation"),
            new DuplicateException("duplicate"),
            new LogicException("logic"),
            new SecurityException("security"),
            new AuthenticationException("authentication"),
            new AuthorizationException("authorization"),
            new UnhandledException("unhandled"),
            new ValidationException("validation", "Email")
        ];

        [Fact]
        public void CreateProblemDetails_ValidationException_WritesDataField()
        {
            var exception = new ValidationException("invalid", "Email");

            var problem = ManagedExceptionProblem.CreateProblemDetails(exception);
            var restored = Assert.IsType<ValidationException>(ManagedExceptionProblem.CreateManagedException(problem));

            Assert.Equal("Email", problem.Extensions[ManagedExceptionProblem.DataFieldExtensionKey]);
            Assert.Equal("Email", restored.DataField);
            Assert.Equal(ManagedExceptionProblem.ValidationExceptionStatus, problem.Status);
        }

        [Fact]
        public void CreateProblemDetails_DataValidationException_WritesPropertyName()
        {
            var exception = new DataValidationException("Amount", "invalid amount");

            var problem = ManagedExceptionProblem.CreateProblemDetails(exception);
            var restored = Assert.IsType<DataValidationException>(ManagedExceptionProblem.CreateManagedException(problem));

            Assert.Equal("Amount", problem.Extensions[ManagedExceptionProblem.PropertyNameExtensionKey]);
            Assert.Equal("Amount", restored.PropertyName);
            Assert.Equal(ManagedExceptionProblem.DataValidationExceptionStatus, problem.Status);
        }

        [Fact]
        public void CreateManagedException_UnknownType_FallsBackToManagedException()
        {
            var problem = new ProblemDetails
            {
                Type = "https://example.test/problems/NoSuchThing",
                Detail = "fallback"
            };

            var restored = ManagedExceptionProblem.CreateManagedException(problem);

            Assert.IsType<ManagedException>(restored);
            Assert.Equal("fallback", restored.Message);
        }

        [Fact]
        public void CreateManagedException_MissingType_FallsBackToManagedException()
        {
            var problem = new ProblemDetails
            {
                Detail = "fallback"
            };

            var restored = ManagedExceptionProblem.CreateManagedException(problem);

            Assert.IsType<ManagedException>(restored);
            Assert.Equal("fallback", restored.Message);
        }

        [Fact]
        public void CreateManagedException_TypeUriIsCaseInsensitiveAndTrimsSlash()
        {
            var problem = new ProblemDetails
            {
                Type = ManagedExceptionProblem.TypeUriBase + "validationexception/",
                Detail = "bad"
            };
            problem.Extensions[ManagedExceptionProblem.DataFieldExtensionKey] = "Email";

            var restored = Assert.IsType<ValidationException>(ManagedExceptionProblem.CreateManagedException(problem));

            Assert.Equal("bad", restored.Message);
            Assert.Equal("Email", restored.DataField);
        }

        [Fact]
        public void CreateManagedException_ReadsPascalCaseExtensionKeys()
        {
            var validationProblem = new ProblemDetails
            {
                Type = ManagedExceptionProblem.TypeUriBase + nameof(ValidationException),
                Detail = "bad"
            };
            validationProblem.Extensions["DataField"] = "Name";

            var dataProblem = new ProblemDetails
            {
                Type = ManagedExceptionProblem.TypeUriBase + nameof(DataValidationException),
                Detail = "bad"
            };
            dataProblem.Extensions["PropertyName"] = "Amount";

            var validation = Assert.IsType<ValidationException>(ManagedExceptionProblem.CreateManagedException(validationProblem));
            var dataValidation = Assert.IsType<DataValidationException>(ManagedExceptionProblem.CreateManagedException(dataProblem));

            Assert.Equal("Name", validation.DataField);
            Assert.Equal("Amount", dataValidation.PropertyName);
        }

        [Fact]
        public void CreateProblemDetails_ManagedExceptions_RoundTripsChildren()
        {
            var exception = new ManagedExceptions(
            [
                new LogicException("logic"),
                new DataException("data")
            ]);

            var problem = ManagedExceptionProblem.CreateProblemDetails(exception);
            var restored = Assert.IsType<ManagedExceptions>(ManagedExceptionProblem.CreateManagedException(problem));

            Assert.Equal(ManagedExceptionProblem.ManagedExceptionsStatus, problem.Status);
            Assert.Equal(2, restored.Exceptions.Count);
            Assert.IsType<LogicException>(restored.Exceptions[0]);
            Assert.Equal("logic", restored.Exceptions[0].Message);
            Assert.IsType<DataException>(restored.Exceptions[1]);
            Assert.Equal("data", restored.Exceptions[1].Message);
        }

        [Fact]
        public void CreateProblemDetails_DropsChildrenBeyondMaxCount()
        {
            var children = new List<ManagedException>(ManagedExceptionProblem.MaxExceptionChildren + 1);
            for (var index = 0; index <= ManagedExceptionProblem.MaxExceptionChildren; index++)
            {
                children.Add(new LogicException(index.ToString()));
            }

            var problem = ManagedExceptionProblem.CreateProblemDetails(new ManagedExceptions(children));
            var restored = Assert.IsType<ManagedExceptions>(ManagedExceptionProblem.CreateManagedException(problem));

            Assert.Equal(ManagedExceptionProblem.MaxExceptionChildren, restored.Exceptions.Count);
        }

        [Fact]
        public void CreateProblemDetails_StopsAtMaxDepth()
        {
            ManagedException current = new LogicException("leaf");
            for (var index = 0; index < ManagedExceptionProblem.MaxExceptionDepth + 3; index++)
            {
                current = new ManagedExceptions([current]);
            }

            var problem = ManagedExceptionProblem.CreateProblemDetails(current);
            var restored = ManagedExceptionProblem.CreateManagedException(problem);

            var depth = 0;
            ManagedException? walk = restored;
            while (walk is ManagedExceptions aggregate)
            {
                depth++;
                Assert.True(depth <= ManagedExceptionProblem.MaxExceptionDepth + 1);
                walk = aggregate.Exceptions.Count == 0 ? null : aggregate.Exceptions[0];
            }

            Assert.True(depth > 0);
        }

        [Fact]
        public void CreateManagedException_JsonElementArray_RestoresChildren()
        {
            var child = ManagedExceptionProblem.CreateProblemDetails(new LogicException("inner"));
            using var document = JsonDocument.Parse(JsonSerializer.Serialize(new[] { child }));
            var problem = new ProblemDetails
            {
                Type = ManagedExceptionProblem.TypeUriBase + nameof(ManagedExceptions)
            };
            problem.Extensions[ManagedExceptionProblem.ExceptionsExtensionKey] = document.RootElement.Clone();

            var restored = Assert.IsType<ManagedExceptions>(ManagedExceptionProblem.CreateManagedException(problem));
            var logic = Assert.IsType<LogicException>(Assert.Single(restored.Exceptions));
            Assert.Equal("inner", logic.Message);
        }

        [Fact]
        public void CreateManagedException_UnknownExtensionObject_IsIgnored()
        {
            var problem = new ProblemDetails
            {
                Type = ManagedExceptionProblem.TypeUriBase + nameof(ManagedExceptions)
            };
            problem.Extensions[ManagedExceptionProblem.ExceptionsExtensionKey] = new object();

            var restored = Assert.IsType<ManagedExceptions>(ManagedExceptionProblem.CreateManagedException(problem));

            Assert.Empty(restored.Exceptions);
        }

        [Fact]
        public void StatusConstants_RemainCustom480Range()
        {
            Assert.Equal(480, ManagedExceptionProblem.ManagedExceptionStatus);
            Assert.Equal(481, ManagedExceptionProblem.ManagedExceptionsStatus);
            Assert.Equal(482, ManagedExceptionProblem.CommunicationExceptionStatus);
            Assert.Equal(483, ManagedExceptionProblem.CommunicationTimeoutExceptionStatus);
            Assert.Equal(484, ManagedExceptionProblem.ConfigurationExceptionStatus);
            Assert.Equal(485, ManagedExceptionProblem.DataExceptionStatus);
            Assert.Equal(486, ManagedExceptionProblem.DataValidationExceptionStatus);
            Assert.Equal(487, ManagedExceptionProblem.DuplicateExceptionStatus);
            Assert.Equal(488, ManagedExceptionProblem.LogicExceptionStatus);
            Assert.Equal(489, ManagedExceptionProblem.SecurityExceptionStatus);
            Assert.Equal(490, ManagedExceptionProblem.AuthenticationExceptionStatus);
            Assert.Equal(491, ManagedExceptionProblem.AuthorizationExceptionStatus);
            Assert.Equal(492, ManagedExceptionProblem.UnhandledExceptionStatus);
            Assert.Equal(493, ManagedExceptionProblem.ValidationExceptionStatus);
        }
    }
}
