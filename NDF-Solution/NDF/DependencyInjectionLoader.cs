//*****************************************************************************
//* Net Delivery Framework
//* Copyright (c) 2022 CodeFactory, LLC
//*****************************************************************************
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NDF
{

    /// <summary>
    /// Base class implementation for a dependency injection class for loading at the library level. 
    /// </summary>
    public abstract class DependencyInjectionLoader
    {
        /// <summary>
        /// Registers services into the hosting dependency injection service provider.
        /// </summary>
        /// <param name="serviceCollection">The dependency injection provider to register services with.</param>
        /// <param name="configuration">The source configuration to provide for dependency injection. </param>
        public void Load(IServiceCollection serviceCollection, IConfiguration configuration)
        {
                LoadLibraries(serviceCollection, configuration);
                LoadManualRegistration(serviceCollection, configuration);
                LoadRegistration(serviceCollection, configuration);
        }

        /// <summary>
        /// Loads child libraries that are subscribed to by this library.
        /// </summary>
        /// <param name="serviceCollection">The dependency injection provider to register services with.</param>
        /// <param name="configuration">The source configuration to provide for dependency injection.</param>
        protected abstract void LoadLibraries(IServiceCollection serviceCollection, IConfiguration configuration);

        /// <summary>
        /// Loads dependency injections that are setup and configured manually.
        /// </summary>
        /// <param name="serviceCollection">The dependency injection provider to register services with.</param>
        /// <param name="configuration">The source configuration to provide for dependency injection.</param>
        protected abstract void LoadManualRegistration(IServiceCollection serviceCollection, IConfiguration configuration);

        /// <summary>
        /// Loads dependency injection registrations.
        /// </summary>
        /// <param name="serviceCollection">The dependency injection provider to register services with.</param>
        /// <param name="configuration">The source configuration to provide for dependency injection.</param>
        protected abstract void LoadRegistration(IServiceCollection serviceCollection, IConfiguration configuration);
    }
}
