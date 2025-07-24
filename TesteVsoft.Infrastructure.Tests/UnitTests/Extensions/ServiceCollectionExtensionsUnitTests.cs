using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TesteVsoft.Application.Interfaces.CQRS;
using TesteVsoft.Infrastructure.Common.EventDispatchers;
using TesteVsoft.Infrastructure.Common.Extensions;
using TesteVsoft.Infrastructure.Tests.Fakes;

namespace TesteVsoft.Infrastructure.Tests.UnitTests.Extensions;

[TestFixture]
public class ServiceCollectionExtensionsUnitTests
{
    private IServiceCollection _services;
    private IConfiguration _configuration;

    private readonly Assembly _applicationAssembly = typeof(TesteVsoft.Infrastructure.Tests.Common.Assemblies.AssemblyReference).Assembly;
    private readonly Assembly _infrastructureAssembly = typeof(TesteVsoft.Infrastructure.Tests.Common.Assemblies.AssemblyReference).Assembly;

    [SetUp]
    public void SetUp()
    {
        _services = new ServiceCollection();

        _configuration = new ConfigurationBuilder()
            .Build();
    }

    [Test]
    public void AddInfrastructureServices_ShouldRegisterAttributedServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        // Act
        services.AddInfrastructureServices(configuration, _applicationAssembly, _infrastructureAssembly);

        var serviceProvider = services.BuildServiceProvider();

        // Act
        var service = serviceProvider.GetService<IFakeScopedService>();

        // Assert
        service.Should().NotBeNull();
        service!.Ping().Should().Be("pong");
    }


    [Test]
    public void AddServicesWithAttributes_ShouldRegisterAllScopedSingletonTransient()
    {
        _services.AddServicesWithAttributes(_infrastructureAssembly);
        var provider = _services.BuildServiceProvider();

        provider.GetService<FakeScopedService>().Should().NotBeNull();
        provider.GetService<IFakeScopedService>().Should().NotBeNull();
        provider.GetService<FakeSingletonService>().Should().NotBeNull();
        provider.GetService<IFakeSingletonService>().Should().NotBeNull();
        provider.GetService<FakeTransientService>().Should().NotBeNull();
        provider.GetService<FakeTransientService>().Should().NotBeNull();
    }

    [Test]
    public void AddEventDispatcher_ShouldRegisterAllHandlersAndResolver()
    {
        // Act
        _services.AddEventDispatcher(_infrastructureAssembly);

        // Assert
        var dispatcherDescriptor = _services.FirstOrDefault(d => d.ServiceType == typeof(IEventDispatcher));
        dispatcherDescriptor.Should().NotBeNull();
        dispatcherDescriptor!.Lifetime.Should().Be(ServiceLifetime.Transient);

        var resolverDescriptor = _services.FirstOrDefault(d => d.ServiceType == typeof(IHandlerResolver));
        resolverDescriptor.Should().NotBeNull();
        resolverDescriptor!.Lifetime.Should().Be(ServiceLifetime.Singleton);
        resolverDescriptor.ImplementationInstance.Should().BeOfType<HandlerResolver>();

        var handlerResolver = (HandlerResolver)resolverDescriptor.ImplementationInstance!;

        handlerResolver.GetHandlerTypesByTypeName(nameof(FakeCommand))
            .Should()
            .NotBeNull();

        handlerResolver.GetHandlerTypesByTypeName(nameof(FakeCommandWithResponse))
            .Should()
            .NotBeNull();

        handlerResolver.GetHandlerTypesByTypeName(nameof(FakeQuery))
            .Should()
            .NotBeNull();

        handlerResolver.GetHandlerTypesByNotificationTypeName(nameof(FakeNotification))
            .Should()
            .HaveCount(2);

        handlerResolver.GetHandlerTypesByNotificationTypeName(nameof(FakeNotification))
            .Should()
            .NotBeNull();

        _services.Should().Contain(sd => sd.ServiceType == typeof(FakeNotificationHandler));
        _services.Should().Contain(sd => sd.ServiceType == typeof(FakeNotificationHandler2));
        _services.Should().Contain(sd => sd.ServiceType == typeof(FakeCommandHandler));
        _services.Should().Contain(sd => sd.ServiceType == typeof(FakeCommandWithResponseHandler));
        _services.Should().Contain(sd => sd.ServiceType == typeof(FakeQueryHandler));
    }
}