using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TesteVsoft.Infrastructure.Common.Extensions;
using TesteVsoft.Infrastructure.Tests.Fakes;

namespace TesteVsoft.Infrastructure.Tests.UnitTests.Extensions;

[TestFixture]
public class ServiceCollectionExtensionsUnitTests
{
    private IServiceCollection _services;
    private IConfiguration _configuration;
    
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
        services.AddInfrastructureServices(configuration, _infrastructureAssembly);

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
}