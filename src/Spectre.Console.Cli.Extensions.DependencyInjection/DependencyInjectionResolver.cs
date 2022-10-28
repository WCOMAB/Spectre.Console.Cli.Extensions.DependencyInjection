using System;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Spectre.Console.Cli.Extensions.DependencyInjection
{
    internal class DependencyInjectionResolver : ITypeResolver, IDisposable
    {
        private ServiceProvider ServiceProvider { get; }

        internal DependencyInjectionResolver(ServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public void Dispose()
        {
            ServiceProvider.Dispose();
        }

        public object Resolve(Type type)
        {
            return ServiceProvider.GetService(type) ?? Activator.CreateInstance(type);
        }
    }
}