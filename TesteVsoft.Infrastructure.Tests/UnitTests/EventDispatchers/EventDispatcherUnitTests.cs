using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TesteVsoft.Application.Interfaces.CQRS;
using TesteVsoft.Infrastructure.Common.EventDispatchers;
using TesteVsoft.Infrastructure.Tests.Fakes;

namespace TesteVsoft.Infrastructure.Tests.UnitTests.EventDispatchers;

[TestFixture]
public class EventDispatcherUnitFakes
{
    private ServiceProvider? _serviceProvider = null;
    private ServiceCollection? _services = null;
    private HandlerResolver? _handlerResolver = null!;
    private EventDispatcher? _dispatcher = null!;

    [SetUp]
    public void Setup()
    {
        _services = new ServiceCollection();
        _handlerResolver = new HandlerResolver();
    }

    private void BuildDispatcher()
    {
        _serviceProvider = _services!.BuildServiceProvider();
        _dispatcher = new EventDispatcher(_serviceProvider, _handlerResolver!);
    }

    [Test]
    public async Task SendAsync_Should_Invoke_Command_Handler()
    {
        // Arrange
        var fakeCommandHandler = new FakeCommandHandler();
        _services!.AddScoped(_ => fakeCommandHandler);
        _handlerResolver!.Register(typeof(FakeCommand), typeof(FakeCommandHandler));
        BuildDispatcher();

        // Act
        await _dispatcher!.SendAsync(new FakeCommand());

        // Assert
        fakeCommandHandler.Handled.Should().BeTrue();
    }
    
    [Test]
    public async Task SendAsync_Should_Return_Response()
    {
        var fakeCommandHandler = new FakeCommandWithResponseHandler();
        _services!.AddScoped(_ => fakeCommandHandler);
        _handlerResolver!.Register(typeof(FakeCommandWithResponse), typeof(FakeCommandWithResponseHandler));
        BuildDispatcher();

        // Act
        var handled = await _dispatcher!.SendAsync<bool>(new FakeCommandWithResponse());

        // Assert
        handled.Should().BeTrue();
    }

    [Test]
    public void SendAsync_Should_Throw_When_Handler_Not_Registered()
    {
        // Arrange
        BuildDispatcher();
        var command = new FakeCommand();

        // Act
        Func<Task> act = async () => await _dispatcher!.SendAsync(command);

        // Assert
        act.Should().ThrowAsync<InvalidCastException>()
            .WithMessage($"*{command.GetType().Name}*");
    }

    [Test]
    public async Task QueryAsync_Should_Return_Response()
    {
        var fakeQueryHandler = new FakeQueryHandler();
        _services!.AddScoped(_ => fakeQueryHandler);
        _handlerResolver!.Register(typeof(FakeQuery), typeof(FakeQueryHandler));
        BuildDispatcher();

        // Act
        var handled = await _dispatcher!.QueryAsync<bool>(new FakeQuery());

        // Assert
        handled.Should().BeTrue();
    }

    [Test]
    public void QueryAsync_Should_Throw_When_Handler_Not_Registered()
    {
        // Arrange
        BuildDispatcher();
        var query = new FakeQuery();

        // Act
        Func<Task> act = async () => await _dispatcher!.QueryAsync<bool>(query);

        // Assert
        act.Should().ThrowAsync<InvalidCastException>()
            .WithMessage($"*{query.GetType().Name}*");
    }

    [Test]
    public async Task NotifyAsync_Should_Invoke_Notification_Handler()
    {
        // Arrange
        var fakeNotificationHandler = new FakeNotificationHandler();
        _services!.AddScoped(_ => fakeNotificationHandler);
        _handlerResolver!.Register(typeof(FakeNotification), typeof(FakeNotificationHandler));
        var fakeNotificationHandler2 = new FakeNotificationHandler2();
        _services!.AddScoped(_ => fakeNotificationHandler2);
        _handlerResolver.Register(typeof(FakeNotification), typeof(FakeNotificationHandler2));
        BuildDispatcher();

        // Act
        await _dispatcher!.NotifyAsync(new FakeNotification());

        // Assert
        fakeNotificationHandler.Handled.Should().BeTrue();
        fakeNotificationHandler2.Handled.Should().BeTrue();
    }

    [Test]
    public void NotifyAsync_Should_Throw_When_Handler_Not_Registered()
    {
        // Arrange
        BuildDispatcher();
        var notification = new FakeNotification();

        // Act
        Func<Task> act = async () => await _dispatcher!.NotifyAsync(notification);

        // Assert
        act.Should().ThrowAsync<InvalidCastException>()
            .WithMessage($"*{notification.GetType().Name}*");
    }

    [Test]
    public async Task NotifyAsync_Should_Continue_When_Handler_Is_Null()
    {
        // Arrange
        var mockHandlerResolver = new Mock<IHandlerResolver>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockScopeFactory = new Mock<IServiceScopeFactory>();
        var mockScope = new Mock<IServiceScope>();
        var mockScopeServiceProvider = new Mock<IServiceProvider>();

        var handlerType = typeof(FakeNotificationHandler);

        mockHandlerResolver
            .Setup(r => r.GetHandlerTypesForNotificationType(typeof(FakeNotification)))
            .Returns(new HashSet<Type> { handlerType });

        mockScopeServiceProvider
            .Setup(p => p.GetService(handlerType))
            .Returns(null!); // Simula handler nulo

        mockScope
            .Setup(s => s.ServiceProvider)
            .Returns(mockScopeServiceProvider.Object);

        mockScopeFactory
            .Setup(f => f.CreateScope())
            .Returns(mockScope.Object);

        mockServiceProvider
            .Setup(p => p.GetService(typeof(IServiceScopeFactory)))
            .Returns(mockScopeFactory.Object);

        var dispatcher = new EventDispatcher(mockServiceProvider.Object, mockHandlerResolver.Object);

        // Act
        var act = async () => await dispatcher.NotifyAsync(new FakeNotification());

        // Assert        
        await act.Should().NotThrowAsync();
    }

    [TearDown]
    public void TearDown()
    {
        _serviceProvider?.Dispose();
        _services = null;
        _handlerResolver = null;
        _dispatcher = null;
    }
}
