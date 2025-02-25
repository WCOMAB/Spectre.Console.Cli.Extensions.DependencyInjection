using System;

namespace Spectre.Console.Cli.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for resolving services from a <see cref="CommandParameterContext"/>.
    /// </summary>
    public static class CommandParameterContextExtensions
    {
        /// <summary>
        /// Gets a service of type <typeparamref name="T"/> from the context's resolver.
        /// </summary>
        /// <typeparam name="T">The type of service to get.</typeparam>
        /// <param name="context">The command parameter context.</param>
        /// <returns>A service of type <typeparamref name="T"/> or null if not found.</returns>
        public static T GetService<T>(this CommandParameterContext context)
            where T : class
            => context?.Resolver.GetService<T>();

        /// <summary>
        /// Gets a required service of type <typeparamref name="T"/> from the context's resolver.
        /// </summary>
        /// <typeparam name="T">The type of service to get.</typeparam>
        /// <param name="context">The command parameter context.</param>
        /// <returns>A service of type <typeparamref name="T"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the service cannot be resolved.</exception>
        public static T GetRequiredService<T>(this CommandParameterContext context)
            where T : class
            => context?.Resolver.GetService<T>() ?? throw new InvalidOperationException($"Failed to resolve {typeof(T)}");
    }
}
