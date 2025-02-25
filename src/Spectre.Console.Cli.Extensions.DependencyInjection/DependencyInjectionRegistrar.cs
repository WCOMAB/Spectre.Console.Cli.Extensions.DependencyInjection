using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Spectre.Console.Cli.Extensions.DependencyInjection
{
    /// <summary>
    /// A type registrar that uses Microsoft.Extensions.DependencyInjection for dependency injection.
    /// </summary>
    public class DependencyInjectionRegistrar : ITypeRegistrar, IDisposable
    {
        private IServiceCollection Services { get; }
        private IList<IDisposable> BuiltProviders { get; }
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyInjectionRegistrar"/> class.
        /// </summary>
        /// <param name="services">The service collection to register services with.</param>
        public DependencyInjectionRegistrar(IServiceCollection services)
        {
            Services = services;
            BuiltProviders = new List<IDisposable>();
        }

        /// <summary>
        /// Builds the type resolver.
        /// </summary>
        /// <returns>A type resolver that can resolve registered services.</returns>
        public ITypeResolver Build()
        {
            var buildServiceProvider = Services.BuildServiceProvider();
            BuiltProviders.Add(buildServiceProvider);
            return new DependencyInjectionResolver(buildServiceProvider);
        }

        /// <summary>
        /// Registers a service type with its implementation type.
        /// </summary>
        /// <param name="service">The service type to register.</param>
        /// <param name="implementation">The implementation type.</param>
        public void Register(Type service, Type implementation)
        {
            Services.AddSingleton(service, implementation);
        }

        /// <summary>
        /// Registers a service type with an implementation instance.
        /// </summary>
        /// <param name="service">The service type to register.</param>
        /// <param name="implementation">The implementation instance.</param>
        public void RegisterInstance(Type service, object implementation)
        {
            Services.AddSingleton(service, implementation);
        }

        /// <summary>
        /// Registers a service type with a factory that creates the implementation.
        /// </summary>
        /// <param name="service">The service type to register.</param>
        /// <param name="factory">The factory that creates the service implementation.</param>
        public void RegisterLazy(Type service, Func<object> factory)
        {
            Services.AddSingleton(service, _ => factory());
        }

        /// <summary>
        /// Disposes all built service providers.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="DependencyInjectionRegistrar"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (var provider in BuiltProviders)
                {
                    provider.Dispose();
                }
            }

            _disposed = true;
        }
    }
}
