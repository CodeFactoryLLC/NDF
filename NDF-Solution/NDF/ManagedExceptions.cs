//*****************************************************************************
//* Net Delivery Framework
//* Copyright (c) 2022 CodeFactory, LLC
//*****************************************************************************
using System.Collections.Generic;
using System.Collections.Immutable;

namespace NDF
{
    /// <summary>
    /// Aggregation of exceptions that inherit from <see cref="ManagedException"/>
    /// </summary>
    public class ManagedExceptions:ManagedException
    {
        /// <summary>
        /// Backing field to hold the managed exceptions.
        /// </summary>
        private readonly ImmutableList<ManagedException> _managedExceptions;


        /// <summary>
        /// Creates a new instance of the <see cref="ManagedExceptions"/>
        /// </summary>
        /// <param name="managedExceptions">Enumeration of the managed exceptions that have occurred.</param>
        public ManagedExceptions(IEnumerable<ManagedException> managedExceptions):base(StandardExceptionMessages.ManagedExceptions) 
        {
            _managedExceptions = managedExceptions != null ? managedExceptions.ToImmutableList() : ImmutableList<ManagedException>.Empty;
        }

        /// <summary>
        /// The exceptions that have occurred.
        /// </summary>
        public IReadOnlyList<ManagedException> Exceptions => _managedExceptions;
    }
}
