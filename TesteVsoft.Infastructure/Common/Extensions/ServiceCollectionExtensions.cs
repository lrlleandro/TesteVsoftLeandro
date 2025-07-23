using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TesteVsoft.Application.Interfaces.CQRS;
using TesteVsoft.Infrastructure.Common.Attributes;
using TesteVsoft.Infrastructure.Common.EventDispatchers;

namespace TesteVsoft.Infrastructure.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, Assembly infrastructureAssembly)
    {
        services.AddServicesWithAttributes(infrastructureAssembly);
        
        return services;
    }

    public static IServiceCollection AddEventDispatcher(this IServiceCollection services, Assembly applicationAssembly)
    {
        var handlerResolver = new HandlerResolver();
        services.AddSingleton<IHandlerResolver>(handlerResolver);
        services.AddTransient<IEventDispatcher, EventDispatcher>();

        var types = applicationAssembly.GetTypes();

        foreach (var notificationHandlerType in types.Where(t => ImplementsGenericInterface(t, typeof(INotificationHandler<>))))
        {
            var notificationType = notificationHandlerType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INotificationHandler<>))
                .GenericTypeArguments[0];

            handlerResolver.Register(notificationType, notificationHandlerType);
            services.AddTransient(notificationHandlerType);
        }

        foreach (var commandHandlerType in types.Where(t => ImplementsGenericInterface(t, typeof(ICommandHandler<>))))
        {
            var commandType = commandHandlerType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>))
                .GenericTypeArguments[0];

            handlerResolver.Register(commandType, commandHandlerType);
            services.AddTransient(commandHandlerType);
        }

        foreach (var commandHandlerType in types.Where(t => ImplementsGenericInterface(t, typeof(ICommandHandler<,>))))
        {
            var commandType = commandHandlerType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))
                .GenericTypeArguments[0];

            handlerResolver.Register(commandType, commandHandlerType);
            services.AddTransient(commandHandlerType);
        }

        foreach (var queryHandlerType in types.Where(t => ImplementsGenericInterface(t, typeof(IQueryHandler<,>))))
        {
            var queryType = queryHandlerType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
                .GenericTypeArguments[0];

            handlerResolver.Register(queryType, queryHandlerType);
            services.AddTransient(queryHandlerType);
        }

        return services;
    }

    public static IServiceCollection AddServicesWithAttributes(this IServiceCollection services, Assembly assembly)
    {
        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(c => c.Where(type => type.GetCustomAttribute<ScopedAttribute>() != null))
            .As(type => type.GetInterfaces())
            .AsSelf()
            .WithScopedLifetime());

        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(c => c.Where(type => type.GetCustomAttribute<SingletonAttribute>() != null))
            .As(type => type.GetInterfaces())
            .AsSelf()
            .WithSingletonLifetime());

        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(c => c.WithAttribute<TransientAttribute>())
            .As(type => type.GetInterfaces())
            .AsSelf()
            .WithTransientLifetime());

        return services;
    }

    private static bool ImplementsGenericInterface(Type type, Type genericInterface)
    {
        return type.GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericInterface);
    }
}
