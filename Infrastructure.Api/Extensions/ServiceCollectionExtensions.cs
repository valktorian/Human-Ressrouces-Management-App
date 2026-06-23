using Infrastructure.Api.Messaging;
using System.Reflection;

namespace Infrastructure.Api.Extensions;

/// <summary>
/// Helper extensions to automatically register command and event handlers found by reflection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Scan loaded assemblies and register:
    /// - All ICommandHandler<TCommand,TResult> as scoped.
    /// - All IEventHandler implementations as scoped.
    /// - ICommandDispatcher as scoped.
    /// </summary>
    public static IServiceCollection AddHandlersFromAssemblies(this IServiceCollection services, params Assembly[]? assemblies)
    {
        var scanAssemblies = (assemblies != null && assemblies.Length > 0)
            ? assemblies
            : AppDomain.CurrentDomain.GetAssemblies();

        // Register command handlers
        var commandHandlerOpen = typeof(ICommandHandler<,>);
        foreach (var type in scanAssemblies.SelectMany(a => SafeGetTypes(a)))
        {
            var interfaces = type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == commandHandlerOpen).ToArray();
            if (interfaces.Length > 0 && !type.IsInterface && !type.IsAbstract)
            {
                foreach (var handlerInterface in interfaces)
                {
                    services.AddScoped(handlerInterface, type);
                }
            }
        }

        // Register event handlers
        var eventHandlerType = typeof(IEventHandler);
        foreach (var type in scanAssemblies.SelectMany(a => SafeGetTypes(a)))
        {
            if (eventHandlerType.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            {
                services.AddScoped(eventHandlerType, type);
                services.AddScoped(type);
            }
        }

        // Register dispatcher
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();

        return services;
    }

    private static IEnumerable<Type> SafeGetTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch
        {
            return Array.Empty<Type>();
        }
    }
}
