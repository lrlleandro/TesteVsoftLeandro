using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TesteVsoft.Infrastructure.Common.Attributes;

namespace TesteVsoft.Infrastructure.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, Assembly infrastructureAssembly)
    {
        services.AddServicesWithAttributes(infrastructureAssembly);
        
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
}
